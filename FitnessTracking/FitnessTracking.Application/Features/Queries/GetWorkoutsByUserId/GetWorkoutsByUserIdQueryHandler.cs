using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public class GetWorkoutsByUserIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetWorkoutsByUserIdQuery, Result<WorkoutListResponse, Error>>
{
    public async Task<Result<WorkoutListResponse, Error>> Handle(
        GetWorkoutsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var workoutsResult = await repository.GetByUserIdAsync(
            request.UserId,
            request.Filter,
            request.SortParameters,
            request.PageParameters,
            cancellationToken);
        
        if (workoutsResult.IsFailure)
        {
            return Result.Failure<WorkoutListResponse, Error>(workoutsResult.Error);
        }

        var totalResult = await repository.GetCountByUserIdAsync(request.UserId, cancellationToken);
        if (totalResult.IsFailure)
        {
            return Result.Failure<WorkoutListResponse, Error>(totalResult.Error);
        }
        var total = totalResult.Value;

        var responses = workoutsResult.Value.Select(MapToResponse).ToList();
        return Result.Success<WorkoutListResponse, Error>(new WorkoutListResponse(responses, total));
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
