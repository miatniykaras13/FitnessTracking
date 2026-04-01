using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public class AddExerciseCommandHandler(
    IWorkoutsRepository repository,
    IValidator<AddExerciseCommand> validator)
    : ICommandHandler<AddExerciseCommand, Result<AddExerciseResponse, List<Error>>>
{
    public async Task<Result<AddExerciseResponse, List<Error>>> Handle(AddExerciseCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<AddExerciseResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<AddExerciseResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var hasDuplicateExercise = workout.Exercises.Any(e =>
            string.Equals(e.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        if (hasDuplicateExercise)
        {
            return Result.Failure<AddExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseAlreadyExists(request.WorkoutId, exercise.Name));
        }

        var addedExercise = await repository.AddExerciseAsync(request.WorkoutId, exercise, cancellationToken);
        if (addedExercise is null)
        {
            return Result.Failure<AddExerciseResponse, List<Error>>(Error.Internal(message: "Failed to add exercise."));
        }

        return Result.Success<AddExerciseResponse, List<Error>>(MapExerciseToResponse(addedExercise));
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
}
