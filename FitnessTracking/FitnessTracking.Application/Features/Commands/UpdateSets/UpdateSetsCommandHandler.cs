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

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(workoutResult.Error);
        }

        if (!string.Equals(workoutResult.Value.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var sets = request.SetDtos.Sets.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();

        var updateResult = await repository.UpdateSetsAsync(
            request.WorkoutId,
            request.ExerciseName,
            sets,
            cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateSetsResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<UpdateSetsResponse, List<Error>>(
            new UpdateSetsResponse(sets.Select(s => new SetDto(s.Reps, s.Weight)).ToList()));
    }
}
