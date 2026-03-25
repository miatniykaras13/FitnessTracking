using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<UpdateExerciseCommand, Result<ExerciseResponse, Error>>
{
    public async Task<Result<ExerciseResponse, Error>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exerciseValidationError = ValidateExerciseFields(request.ExerciseDto.Name, request.ExerciseDto.Sets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<ExerciseResponse, Error>(exerciseValidationError);
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var updateResult = await repository.UpdateExerciseAsync(request.WorkoutId, request.ExerciseName, exercise, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(updateResult.Error);
        }

        return Result.Success<ExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    private static ExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return new ExerciseResponse(exercise.Name, setResponses);
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

    private static Error? ValidateExerciseFields(string? name, IReadOnlyList<SetDto>? sets)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return WorkoutErrors.ExerciseNameRequired();
        }

        if (sets is null)
        {
            return WorkoutErrors.ExerciseSetsRequired();
        }

        foreach (var set in sets)
        {
            var setValidationError = ValidateSetFields(set.Reps, set.Weight);
            if (setValidationError is not null)
            {
                return setValidationError;
            }
        }

        return null;
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
