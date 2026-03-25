using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchSet;

public class PatchSetCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper)
    : ICommandHandler<PatchSetCommand, Result<SetResponse, Error>>
{
    public async Task<Result<SetResponse, Error>> Handle(PatchSetCommand request, CancellationToken cancellationToken)
    {
        if (request.SetIndex < 0)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        if (request.SetIndex >= exercise.Sets.Count)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetNotFound(request.WorkoutId, request.ExerciseName, request.SetIndex));
        }

        var existingSet = exercise.Sets[request.SetIndex];
        var currentDto = new MergePatchSetDto
        {
            Reps = existingSet.Reps,
            Weight = existingSet.Weight
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);
        if (!patchedDto.Reps.HasValue)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetRepsMustBePositive());
        }

        if (!patchedDto.Weight.HasValue)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetWeightMustBeNonNegative());
        }

        var setValidationError = ValidateSetFields(patchedDto.Reps.Value, patchedDto.Weight.Value);
        if (setValidationError is not null)
        {
            return Result.Failure<SetResponse, Error>(setValidationError);
        }

        existingSet.Reps = patchedDto.Reps.Value;
        existingSet.Weight = patchedDto.Weight.Value;

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(updateResult.Error);
        }

        return Result.Success<SetResponse, Error>(new SetResponse(existingSet.Reps, existingSet.Weight));
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
