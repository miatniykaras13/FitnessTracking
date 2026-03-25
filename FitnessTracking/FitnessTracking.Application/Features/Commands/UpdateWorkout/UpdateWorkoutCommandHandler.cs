using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateWorkout;

public class UpdateWorkoutCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<UpdateWorkoutCommand, Result<UpdateWorkoutResponse, Error>>
{
    public async Task<Result<UpdateWorkoutResponse, Error>> Handle(UpdateWorkoutCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(WorkoutErrors.UserIdRequired());
        }

        var workoutValidationError = ValidateWorkoutFields(
            request.WorkoutDto.Title,
            request.WorkoutDto.Type,
            request.WorkoutDto.Duration,
            request.WorkoutDto.CaloriesBurned,
            request.WorkoutDto.WorkoutDate);
        if (workoutValidationError is not null)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(workoutValidationError);
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;

        workout.Title = request.WorkoutDto.Title;
        workout.Type = workoutType;
        workout.Duration = request.WorkoutDto.Duration;
        workout.CaloriesBurned = request.WorkoutDto.CaloriesBurned;
        workout.WorkoutDate = request.WorkoutDto.WorkoutDate;

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(updateResult.Error);
        }

        return Result.Success<UpdateWorkoutResponse, Error>(MapToResponse(workout));
    }

    private static UpdateWorkoutResponse MapToResponse(Domain.Models.Workout workout)
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

    private static Error? ValidateWorkoutFields(
        string? title,
        string? type,
        TimeSpan duration,
        int caloriesBurned,
        DateTime workoutDate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return WorkoutErrors.TitleRequired();
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            return WorkoutErrors.TypeRequired();
        }

        if (duration <= TimeSpan.Zero)
        {
            return WorkoutErrors.DurationMustBePositive();
        }

        if (caloriesBurned < 0)
        {
            return WorkoutErrors.CaloriesBurnedMustBeNonNegative();
        }

        if (workoutDate == default)
        {
            return WorkoutErrors.WorkoutDateRequired();
        }

        return null;
    }
}
