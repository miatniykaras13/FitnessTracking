using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkout;

public class DeleteWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IValidator<DeleteWorkoutCommand> validator)
    : ICommandHandler<DeleteWorkoutCommand, UnitResult<List<Error>>>
{
    public async Task<UnitResult<List<Error>>> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnitResult.Failure(validationResult.Errors.ToErrors(nameof(Workout).ToLower()));
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var isDeleted = await repository.DeleteAsync(request.WorkoutId, cancellationToken);
        if (!isDeleted)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        return UnitResult.Success<List<Error>>();
    }
}
