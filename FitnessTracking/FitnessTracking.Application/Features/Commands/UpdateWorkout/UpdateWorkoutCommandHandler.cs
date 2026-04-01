using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateWorkout;

public class UpdateWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateWorkoutCommand> validator)
    : ICommandHandler<UpdateWorkoutCommand, Result<UpdateWorkoutResponse, List<Error>>>
{
    public async Task<Result<UpdateWorkoutResponse, List<Error>>> Handle(
        UpdateWorkoutCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<UpdateWorkoutResponse, List<Error>>(
                WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<UpdateWorkoutResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<UpdateWorkoutResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        workout.Title = request.WorkoutDto.Title;
        workout.Type = workoutType;
        workout.Duration = request.WorkoutDto.Duration;
        workout.CaloriesBurned = request.WorkoutDto.CaloriesBurned;
        workout.WorkoutDate = request.WorkoutDto.WorkoutDate;

        var isUpdated = await repository.UpdateAsync(workout, cancellationToken);
        if (!isUpdated)
        {
            return Result.Failure<UpdateWorkoutResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        return Result.Success<UpdateWorkoutResponse, List<Error>>(MapToResponse(workout));
    }

    private static UpdateWorkoutResponse MapToResponse(Workout workout)
    {
        return new UpdateWorkoutResponse(
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
