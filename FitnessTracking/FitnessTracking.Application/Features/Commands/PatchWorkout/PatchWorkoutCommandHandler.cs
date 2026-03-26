using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public class PatchWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper,
    IValidator<PatchWorkoutCommand> validator,
    IValidator<MergePatchWorkoutDto> mergePatchValidator)
    : ICommandHandler<PatchWorkoutCommand, Result<PatchWorkoutResponse, List<Error>>>
{
    public async Task<Result<PatchWorkoutResponse, List<Error>>> Handle(
        PatchWorkoutCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<PatchWorkoutResponse, List<Error>>(workoutResult.Error);
        }

        var workout = workoutResult.Value;

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<PatchWorkoutResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var currentDto = new MergePatchWorkoutDto()
        {
            Title = workout.Title,
            Type = workout.Type.ToString(),
            Duration = workout.Duration,
            CaloriesBurned = workout.CaloriesBurned,
            WorkoutDate = workout.WorkoutDate
        };
        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var patchValidationResult = await mergePatchValidator.ValidateAsync(patchedDto, cancellationToken);
        if (!patchValidationResult.IsValid)
        {
            return patchValidationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var title = patchedDto.Title!;
        var type = patchedDto.Type!;
        var duration = patchedDto.Duration!.Value;
        var caloriesBurned = patchedDto.CaloriesBurned!.Value;
        var workoutDate = patchedDto.WorkoutDate!.Value;

        if (!Enum.TryParse<WorkoutType>(type, true, out var workoutType))
        {
            return Result.Failure<PatchWorkoutResponse, List<Error>>(WorkoutErrors.InvalidWorkoutType(type));
        }

        workout.Title = title;
        workout.Type = workoutType;
        workout.Duration = duration;
        workout.CaloriesBurned = caloriesBurned;
        workout.WorkoutDate = workoutDate;

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);

        if (updateResult.IsFailure)
        {
            return Result.Failure<PatchWorkoutResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<PatchWorkoutResponse, List<Error>>(MapToResponse(workout));
    }

    private static PatchWorkoutResponse MapToResponse(Workout workout)
    {
        return new PatchWorkoutResponse(
            Guid.Parse(workout.Id),
            Guid.Parse(workout.UserId),
            workout.Title,
            workout.Type.ToString(),
            workout.Duration,
            workout.CaloriesBurned,
            workout.WorkoutDate,
            workout.CreatedAt);
    }
}
