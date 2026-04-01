using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddSet;

public class AddSetCommandHandler(
    IWorkoutsRepository repository,
    IValidator<AddSetCommand> validator)
    : ICommandHandler<AddSetCommand, Result<AddSetResponse, List<Error>>>
{
    public async Task<Result<AddSetResponse, List<Error>>> Handle(AddSetCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<AddSetResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<AddSetResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var exerciseExists = workout.Exercises.Any(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (!exerciseExists)
        {
            return Result.Failure<AddSetResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var addedSet = await repository.AddSetAsync(request.WorkoutId, request.ExerciseName, set, cancellationToken);
        if (addedSet is null)
        {
            return Result.Failure<AddSetResponse, List<Error>>(Error.Internal(message: "Failed to add set."));
        }

        return new AddSetResponse(addedSet.Reps, addedSet.Weight);
    }
}
