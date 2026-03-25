using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public class GetWorkoutByIdQueryHandler(
    IWorkoutsRepository repository,
    IValidator<GetWorkoutByIdQuery> validator)
    : IQueryHandler<GetWorkoutByIdQuery, Result<GetWorkoutByIdResponse, List<Error>>>
{
    public async Task<Result<GetWorkoutByIdResponse, List<Error>>> Handle(
        GetWorkoutByIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workoutResult = await repository.GetByIdWithPhotosAsync(request.WorkoutId, cancellationToken);

        if (workoutResult.IsFailure)
        {
            return Result.Failure<GetWorkoutByIdResponse, List<Error>>(workoutResult.Error);
        }

        return Result.Success<GetWorkoutByIdResponse, List<Error>>(MapToResponse(workoutResult.Value));
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
            workout.CreatedAt,
            workout.ProgressPhotos.Select(p => p.Id));
    }
}
