using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateExerciseCommand> validator)
    : ICommandHandler<UpdateExerciseCommand, Result<UpdateExerciseResponse, List<Error>>>
{
    public async Task<Result<UpdateExerciseResponse, List<Error>>> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(workoutResult.Error);
        }

        if (!string.Equals(workoutResult.Value.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var workout = workoutResult.Value;
        var existingExercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (existingExercise is null)
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var hasNameConflict = workout.Exercises.Any(e =>
            !string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(e.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        if (hasNameConflict)
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseAlreadyExists(request.WorkoutId, exercise.Name));
        }

        var isUpdated = await repository.UpdateExerciseAsync(request.WorkoutId, request.ExerciseName, exercise, cancellationToken);
        if (!isUpdated)
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        return Result.Success<UpdateExerciseResponse, List<Error>>(MapExerciseToResponse(exercise));
    }

    private static UpdateExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new UpdateExerciseResponse(exercise.Name, setResponses);
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

}
