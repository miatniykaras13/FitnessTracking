using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public class AddExerciseCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<AddExerciseCommand, Result<AddExerciseResponse, Error>>
{
    public async Task<Result<AddExerciseResponse, Error>> Handle(AddExerciseCommand request, CancellationToken cancellationToken)
    {
        var exerciseValidationError = ValidateExerciseFields(request.ExerciseDto.Name, request.ExerciseDto.Sets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<AddExerciseResponse, Error>(exerciseValidationError);
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var addResult = await repository.AddExerciseAsync(request.WorkoutId, exercise, cancellationToken);
        if (addResult.IsFailure)
        {
            return Result.Failure<AddExerciseResponse, Error>(addResult.Error);
        }

        return Result.Success<AddExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    private static AddExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new AddExerciseResponse(exercise.Name, setResponses);
    }

    private static List<Set> MapSetDtos(IReadOnlyList<AddSetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

    private static Error? ValidateExerciseFields(string? name, IReadOnlyList<AddSetDto>? sets)
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
