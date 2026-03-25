using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public class PatchWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper)
    : ICommandHandler<PatchWorkoutCommand, Result<PatchWorkoutResponse, Error>>
{
    public async Task<Result<PatchWorkoutResponse, Error>> Handle(PatchWorkoutCommand request, CancellationToken cancellationToken)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<PatchWorkoutResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;

        var currentDto = new MergePatchWorkoutDto()
        {
            Title = workout.Title,
            Type = workout.Type.ToString(),
            Duration = workout.Duration,
            CaloriesBurned = workout.CaloriesBurned,
            WorkoutDate = workout.WorkoutDate
        };
        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var title = patchedDto.Title;
        var type = patchedDto.Type;
        var duration = patchedDto.Duration;
        var caloriesBurned = patchedDto.CaloriesBurned;
        var workoutDate = patchedDto.WorkoutDate;

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.TitleRequired());
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.TypeRequired());
        }

        if (duration is null || duration.Value <= TimeSpan.Zero)
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.DurationMustBePositive());
        }

        if (caloriesBurned is null or < 0)
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.CaloriesBurnedMustBeNonNegative());
        }

        if (workoutDate is null || workoutDate.Value == default)
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.WorkoutDateRequired());
        }

        if (!Enum.TryParse<WorkoutType>(type, true, out var workoutType))
        {
            return Result.Failure<PatchWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(type));
        }

        workout.Title = title;
        workout.Type = workoutType;
        workout.Duration = duration.Value;
        workout.CaloriesBurned = caloriesBurned.Value;
        workout.WorkoutDate = workoutDate.Value;

        var updateResult = await repository.UpdateAsync(workout, cancellationToken);

        if (updateResult.IsFailure)
        {
            return Result.Failure<PatchWorkoutResponse, Error>(updateResult.Error);
        }

        return Result.Success<PatchWorkoutResponse, Error>(MapToResponse(workout));
    }

    private static PatchWorkoutResponse MapToResponse(Domain.Models.Workout workout)
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
