using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
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

        var result = await repository.DeleteSetAsync(request.WorkoutId, request.ExerciseName, request.SetIndex, cancellationToken);
        if (result.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(result.Error);
        }

        return UnitResult.Success<List<Error>>();
    }
}
