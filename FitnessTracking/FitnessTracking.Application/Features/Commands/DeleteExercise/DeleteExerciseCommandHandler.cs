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

        var result = await repository.DeleteExerciseAsync(request.WorkoutId, request.ExerciseName, cancellationToken);
        if (result.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(result.Error);
        }

        return UnitResult.Success<List<Error>>();
    }
}
