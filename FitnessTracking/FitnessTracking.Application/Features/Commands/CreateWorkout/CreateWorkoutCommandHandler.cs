using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.CreateWorkout;

public class CreateWorkoutCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<CreateWorkoutCommand, Result<CreateWorkoutResponse, Error>>
{
    public async Task<Result<CreateWorkoutResponse, Error>> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(WorkoutErrors.UserIdRequired());
        }

        var workoutValidationError = ValidateWorkoutFields(
            request.WorkoutDto.Title,
            request.WorkoutDto.Type,
            request.WorkoutDto.Duration,
            request.WorkoutDto.CaloriesBurned,
            request.WorkoutDto.WorkoutDate);
        if (workoutValidationError is not null)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(workoutValidationError);
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<CreateWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
        }

        var workoutId = Guid.NewGuid();
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = request.UserId.ToString(),
            Title = request.WorkoutDto.Title,
            Type = workoutType,
            Duration = request.WorkoutDto.Duration,
            CaloriesBurned = request.WorkoutDto.CaloriesBurned,
            WorkoutDate = request.WorkoutDto.WorkoutDate,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await repository.AddAsync(workout, cancellationToken);

        if (addResult.IsFailure)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(addResult.Error);
        }

        var response = new CreateWorkoutResponse(
            Guid.Parse(workout.Id),
            Guid.Parse(workout.UserId),
            workout.Title,
            workout.Type.ToString(),
            workout.Duration,
            workout.CaloriesBurned,
            workout.WorkoutDate,
            workout.CreatedAt);

        return Result.Success<CreateWorkoutResponse, Error>(response);
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