using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public class GetWorkoutByIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetWorkoutByIdQuery, Result<GetWorkoutByIdResponse, Error>>
{
    public async Task<Result<GetWorkoutByIdResponse, Error>> Handle(
        GetWorkoutByIdQuery request,
        CancellationToken cancellationToken)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);

        if (workoutResult.IsFailure)
        {
            return Result.Failure<GetWorkoutByIdResponse, Error>(workoutResult.Error);
        }

        return Result.Success<GetWorkoutByIdResponse, Error>(MapToResponse(workoutResult.Value));
    }

    private static GetWorkoutByIdResponse MapToResponse(Workout workout)
    {
        return new GetWorkoutByIdResponse(
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
