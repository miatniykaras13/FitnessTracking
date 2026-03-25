using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
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

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var updateResult = await repository.UpdateSetAsync(
            request.WorkoutId,
            request.ExerciseName,
            request.SetIndex,
            set,
            cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateSetResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<UpdateSetResponse, List<Error>>(new UpdateSetResponse(set.Reps, set.Weight));
    }
}
