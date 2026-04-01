using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Constants;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateSet;

public class UpdateSetCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateSetCommand> validator)
    : ICommandHandler<UpdateSetCommand, Result<UpdateSetResponse, List<Error>>>
{
    public async Task<Result<UpdateSetResponse, List<Error>>> Handle(
        UpdateSetCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Set).ToLower());
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        if (request.SetIndex < ValidationConstants.MinZeroBasedIndex || request.SetIndex >= exercise.Sets.Count)
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(
                WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var isUpdated = await repository.UpdateSetAsync(
            request.WorkoutId,
            request.ExerciseName,
            request.SetIndex,
            set,
            cancellationToken);
        if (!isUpdated)
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(
                WorkoutErrors.SetNotFound(request.WorkoutId, request.ExerciseName, request.SetIndex));
        }

        return Result.Success<UpdateSetResponse, List<Error>>(new UpdateSetResponse(set.Reps, set.Weight));
    }
}
