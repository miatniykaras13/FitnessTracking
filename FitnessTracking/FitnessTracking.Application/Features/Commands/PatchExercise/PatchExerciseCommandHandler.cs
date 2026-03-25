using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchExercise;

public class PatchExerciseCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper)
    : ICommandHandler<PatchExerciseCommand, Result<ExerciseResponse, Error>>
{
    public async Task<Result<ExerciseResponse, Error>> Handle(PatchExerciseCommand request, CancellationToken cancellationToken)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<ExerciseResponse, Error>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var currentDto = new MergePatchExerciseDto
        {
            Name = exercise.Name,
            Sets = exercise.Sets.Select(s => new AddSetDto(s.Reps, s.Weight)).ToList()
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var patchedName = patchedDto.Name;
        var patchedSets = patchedDto.Sets;

        var exerciseValidationError = ValidateExerciseFields(patchedName, patchedSets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<ExerciseResponse, Error>(exerciseValidationError);
        }

        var hasNameConflict = workout.Exercises.Any(e =>
            !ReferenceEquals(e, exercise) &&
            string.Equals(e.Name, patchedName, StringComparison.OrdinalIgnoreCase));
        if (hasNameConflict)
        {
            return Result.Failure<ExerciseResponse, Error>(WorkoutErrors.ExerciseAlreadyExists(request.WorkoutId, patchedName!));
        }

        exercise.Name = patchedName!;
        exercise.Sets = patchedSets!.Select(s => new Set { Reps = s.Reps, Weight = s.Weight }).ToList();

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(updateResult.Error);
        }

        return Result.Success<ExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    private static ExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return new ExerciseResponse(exercise.Name, setResponses);
    }

    private static Error? ValidateExerciseFields(string? name, IReadOnlyList<AddSetDto>? sets)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return WorkoutErrors.ExerciseNameRequired();
        }

        if (sets is null)
        {
            return WorkoutErrors.ExerciseSetsRequired();
        }

        foreach (var set in sets)
        {
            var setValidationError = ValidateSetFields(set.Reps, set.Weight);
            if (setValidationError is not null)
            {
                return setValidationError;
            }
        }

        return null;
    }

    private static Error? ValidateSetFields(int reps, double weight)
    {
        if (reps <= 0)
        {
            return WorkoutErrors.SetRepsMustBePositive();
        }

        if (weight < 0)
        {
            return WorkoutErrors.SetWeightMustBeNonNegative();
        }

        return null;
    }
}
