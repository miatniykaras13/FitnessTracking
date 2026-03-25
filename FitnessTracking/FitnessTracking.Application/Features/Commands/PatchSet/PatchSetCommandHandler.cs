using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchSet;

public class PatchSetCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper,
    IValidator<PatchSetCommand> validator,
    IValidator<MergePatchSetDto> mergePatchValidator)
    : ICommandHandler<PatchSetCommand, Result<PatchSetResponse, List<Error>>>
{
    public async Task<Result<PatchSetResponse, List<Error>>> Handle(
        PatchSetCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Set).ToLower());
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<PatchSetResponse, List<Error>>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<PatchSetResponse, List<Error>>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        if (request.SetIndex >= exercise.Sets.Count)
        {
            return Result.Failure<PatchSetResponse, List<Error>>(WorkoutErrors.SetNotFound(request.WorkoutId, request.ExerciseName, request.SetIndex));
        }

        var existingSet = exercise.Sets[request.SetIndex];
        var currentDto = new MergePatchSetDto
        {
            Reps = existingSet.Reps,
            Weight = existingSet.Weight
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);
        var patchValidationResult = await mergePatchValidator.ValidateAsync(patchedDto, cancellationToken);
        if (!patchValidationResult.IsValid)
        {
            return patchValidationResult.Errors.ToErrors(nameof(Set).ToLower());
        }

        existingSet.Reps = patchedDto.Reps!.Value;
        existingSet.Weight = patchedDto.Weight!.Value;

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<PatchSetResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<PatchSetResponse, List<Error>>(new PatchSetResponse(existingSet.Reps, existingSet.Weight));
    }
}
