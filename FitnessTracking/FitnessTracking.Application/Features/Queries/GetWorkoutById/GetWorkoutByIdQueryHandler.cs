using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public class GetWorkoutByIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetWorkoutByIdQuery, Result<WorkoutResponse, Error>>
{
    public async Task<Result<WorkoutResponse, Error>> Handle(
        GetWorkoutByIdQuery request,
        CancellationToken cancellationToken)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);

        if (workoutResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutResult.Error);
        }

        return Result.Success<WorkoutResponse, Error>(MapToResponse(workoutResult.Value));
    }

    private static WorkoutResponse MapToResponse(Workout workout)
    {
        return new WorkoutResponse(
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
