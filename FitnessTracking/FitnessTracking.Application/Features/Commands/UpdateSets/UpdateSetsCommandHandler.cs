using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public class UpdateSetsCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<UpdateSetsCommand, Result<SetListResponse, Error>>
{
    public async Task<Result<SetListResponse, Error>> Handle(UpdateSetsCommand request, CancellationToken cancellationToken)
    {
        foreach (var setDto in request.SetDtos.Sets)
        {
            var setValidationError = ValidateSetFields(setDto.Reps, setDto.Weight);
            if (setValidationError is not null)
            {
                return Result.Failure<SetListResponse, Error>(setValidationError);
            }
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
            return Result.Failure<SetListResponse, Error>(updateResult.Error);
        }

        return Result.Success<SetListResponse, Error>(
            new SetListResponse(sets.Select(s => new SetResponse(s.Reps, s.Weight)).ToList()));
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
