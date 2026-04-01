using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public class UpdateSetsCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateSetsCommand> validator)
    : ICommandHandler<UpdateSetsCommand, Result<UpdateSetsResponse, List<Error>>>
{
    public async Task<Result<UpdateSetsResponse, List<Error>>> Handle(
        UpdateSetsCommand request,
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
            return Result.Failure<UpdateSetsResponse, List<Error>>(
                WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var exerciseExists = workout.Exercises.Any(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (!exerciseExists)
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var sets = request.SetDtos.Sets.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();

        var isUpdated = await repository.UpdateSetsAsync(
            request.WorkoutId,
            request.ExerciseName,
            sets,
            cancellationToken);
        if (!isUpdated)
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        return Result.Success<UpdateSetsResponse, List<Error>>(
            new UpdateSetsResponse(sets.Select(s => new SetDto(s.Reps, s.Weight)).ToList()));
    }
}
