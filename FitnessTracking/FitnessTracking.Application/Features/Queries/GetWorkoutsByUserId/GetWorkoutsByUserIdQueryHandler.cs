using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public class GetWorkoutsByUserIdQueryHandler(
    IWorkoutsRepository repository,
    IValidator<GetWorkoutsByUserIdQuery> validator)
    : IQueryHandler<GetWorkoutsByUserIdQuery, Result<GetWorkoutsByUserIdResponse, List<Error>>>
{
    public async Task<Result<GetWorkoutsByUserIdResponse, List<Error>>> Handle(
        GetWorkoutsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workouts = await repository.GetByUserIdAsync(
            request.UserId,
            request.Filter,
            request.SortParameters,
            request.PageParameters,
            cancellationToken);


        var total = await repository.GetCountByUserIdWithFilterAsync(
            request.UserId,
            request.Filter,
            cancellationToken);

        var responses = workouts.Select(MapToResponse).ToList();
        return Result.Success<GetWorkoutsByUserIdResponse, List<Error>>(
            new GetWorkoutsByUserIdResponse(responses, total));
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
