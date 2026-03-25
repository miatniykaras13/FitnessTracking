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

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var addResult = await repository.AddSetAsync(request.WorkoutId, request.ExerciseName, set, cancellationToken);
        if (addResult.IsFailure)
        {
            return Result.Failure<AddSetResponse, List<Error>>(addResult.Error);
        }

        return new AddSetResponse(set.Reps, set.Weight);
    }
}
