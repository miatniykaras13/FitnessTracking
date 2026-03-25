using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public class GetWorkoutsByUserIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetWorkoutsByUserIdQuery, Result<GetWorkoutsByUserIdResponse, Error>>
{
    public async Task<Result<GetWorkoutsByUserIdResponse, Error>> Handle(
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
            return Result.Failure<GetWorkoutsByUserIdResponse, Error>(workoutsResult.Error);
        }

        var totalResult = await repository.GetCountByUserIdAsync(request.UserId, cancellationToken);
        if (totalResult.IsFailure)
        {
            return Result.Failure<GetWorkoutsByUserIdResponse, Error>(totalResult.Error);
        }
        var total = totalResult.Value;

        var responses = workoutsResult.Value.Select(MapToResponse).ToList();
        return Result.Success<GetWorkoutsByUserIdResponse, Error>(new GetWorkoutsByUserIdResponse(responses, total));
    }

    private static WorkoutNoUserIdDto MapToResponse(Workout workout)
    {
        return new WorkoutNoUserIdDto(
            Guid.Parse(workout.Id),
            workout.Title,
            workout.Type.ToString(),
            workout.Duration,
            workout.CaloriesBurned,
            workout.WorkoutDate,
            workout.CreatedAt);
    }
}
