using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchExercise;

public class PatchExerciseCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper,
    IValidator<PatchExerciseCommand> validator,
    IValidator<MergePatchExerciseDto> mergePatchValidator)
    : ICommandHandler<PatchExerciseCommand, Result<PatchExerciseResponse, List<Error>>>
{
    public async Task<Result<PatchExerciseResponse, List<Error>>> Handle(
        PatchExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<PatchExerciseResponse, List<Error>>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<PatchExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var currentDto = new MergePatchExerciseDto
        {
            Name = exercise.Name,
            Sets = exercise.Sets.Select(s => new AddSetDto(s.Reps, s.Weight)).ToList()
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var patchedName = patchedDto.Name;
        var patchedSets = patchedDto.Sets;

        var patchValidationResult = await mergePatchValidator.ValidateAsync(patchedDto, cancellationToken);
        if (!patchValidationResult.IsValid)
        {
            return patchValidationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var hasNameConflict = workout.Exercises.Any(e =>
            !ReferenceEquals(e, exercise) &&
            string.Equals(e.Name, patchedName, StringComparison.OrdinalIgnoreCase));
        if (hasNameConflict)
        {
            return Result.Failure<PatchExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseAlreadyExists(request.WorkoutId, patchedName!));
        }

        exercise.Name = patchedName!;
        exercise.Sets = patchedSets!.Select(s => new Set { Reps = s.Reps, Weight = s.Weight }).ToList();

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<PatchExerciseResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<PatchExerciseResponse, List<Error>>(MapExerciseToResponse(exercise));
    }

    private static PatchExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new PatchExerciseResponse(exercise.Name, setResponses);
    }
}