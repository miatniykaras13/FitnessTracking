using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Constants;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteSet;

public class DeleteSetCommandHandler(
    IWorkoutsRepository repository,
    IValidator<DeleteSetCommand> validator)
    : ICommandHandler<DeleteSetCommand, UnitResult<List<Error>>>
{
    public async Task<UnitResult<List<Error>>> Handle(DeleteSetCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnitResult.Failure(validationResult.Errors.ToErrors(nameof(Set).ToLower()));
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

        var exercise = workoutResult.Value.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        if (request.SetIndex < ValidationConstants.MinZeroBasedIndex || request.SetIndex >= exercise.Sets.Count)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        var isDeleted = await repository.DeleteSetAsync(request.WorkoutId, request.ExerciseName, request.SetIndex, cancellationToken);
        if (!isDeleted)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.SetNotFound(request.WorkoutId, request.ExerciseName, request.SetIndex));
        }

        return UnitResult.Success<List<Error>>();
    }
}
