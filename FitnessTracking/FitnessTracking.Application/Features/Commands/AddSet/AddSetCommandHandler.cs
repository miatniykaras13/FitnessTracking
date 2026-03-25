using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddSet;

public class AddSetCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<AddSetCommand, Result<SetResponse, Error>>
{
    public async Task<Result<SetResponse, Error>> Handle(AddSetCommand request, CancellationToken cancellationToken)
    {
        var setValidationError = ValidateSetFields(request.SetDto.Reps, request.SetDto.Weight);
        if (setValidationError is not null)
        {
            return Result.Failure<SetResponse, Error>(setValidationError);
        }

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var addResult = await repository.AddSetAsync(request.WorkoutId, request.ExerciseName, set, cancellationToken);
        if (addResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(addResult.Error);
        }

        return Result.Success<SetResponse, Error>(new SetResponse(set.Reps, set.Weight));
    }

    private static Error? ValidateSetFields(int reps, double weight)
    {
        if (reps <= 0)
        {
            return WorkoutErrors.SetRepsMustBePositive();
        }

        if (weight < 0)
        {
            return WorkoutErrors.SetWeightMustBeNonNegative();
        }

        return null;
    }
}
