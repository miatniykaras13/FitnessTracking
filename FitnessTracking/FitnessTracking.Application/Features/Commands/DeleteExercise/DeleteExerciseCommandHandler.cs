using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteExercise;

public class DeleteExerciseCommandHandler(
    IWorkoutsRepository repository,
    IValidator<DeleteExerciseCommand> validator)
    : ICommandHandler<DeleteExerciseCommand, UnitResult<List<Error>>>
{
    public async Task<UnitResult<List<Error>>> Handle(
        DeleteExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnitResult.Failure(validationResult.Errors.ToErrors(nameof(Exercise).ToLower()));
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

        var hasExercise = workoutResult.Value.Exercises.Any(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (!hasExercise)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var isDeleted = await repository.DeleteExerciseAsync(request.WorkoutId, request.ExerciseName, cancellationToken);
        if (!isDeleted)
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        return UnitResult.Success<List<Error>>();
    }
}
