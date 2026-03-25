using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public class UpdateExercisesCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<UpdateExercisesCommand, Result<UpdateExercisesResponse, Error>>
{
    public async Task<Result<UpdateExercisesResponse, Error>> Handle(UpdateExercisesCommand request, CancellationToken cancellationToken)
    {
        foreach (var exerciseDto in request.ExerciseDtos.Exercises)
        {
            var exerciseValidationError = ValidateExerciseFields(exerciseDto.Name, exerciseDto.Sets);
            if (exerciseValidationError is not null)
            {
                return Result.Failure<UpdateExercisesResponse, Error>(exerciseValidationError);
            }
        }

        var exercises = request.ExerciseDtos.Exercises.Select(MapExerciseDtoToDomain).ToList();

        var updateResult = await repository.UpdateExercisesAsync(request.WorkoutId, exercises, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateExercisesResponse, Error>(updateResult.Error);
        }

        var response = exercises.Select(MapExerciseToResponse).ToList();
        return Result.Success<UpdateExercisesResponse, Error>(new UpdateExercisesResponse(response));
    }

    private static Exercise MapExerciseDtoToDomain(UpdateExerciseDto dto)
    {
        return new Exercise
        {
            Name = dto.Name,
            Sets = MapSetDtos(dto.Sets)
        };
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

    private static ExerciseDto MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new ExerciseDto(exercise.Name, setResponses);
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
