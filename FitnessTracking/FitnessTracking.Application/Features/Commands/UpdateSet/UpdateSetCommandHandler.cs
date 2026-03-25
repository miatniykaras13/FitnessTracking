using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateSet;

public class UpdateSetCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<UpdateSetCommand, Result<SetResponse, Error>>
{
    public async Task<Result<SetResponse, Error>> Handle(UpdateSetCommand request, CancellationToken cancellationToken)
    {
        if (request.SetIndex < 0)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

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

        var updateResult = await repository.UpdateSetAsync(
            request.WorkoutId,
            request.ExerciseName,
            request.SetIndex,
            set,
            cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(updateResult.Error);
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
