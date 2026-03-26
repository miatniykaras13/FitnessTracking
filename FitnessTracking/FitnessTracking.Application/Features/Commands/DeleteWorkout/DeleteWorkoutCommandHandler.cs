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

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(workoutResult.Error);
        }

        if (!string.Equals(workoutResult.Value.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var result = await repository.DeleteAsync(request.WorkoutId, cancellationToken);
        if (result.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(result.Error);
        }

        return UnitResult.Success<List<Error>>();
    }
}
