using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public class GetExercisesByWorkoutIdQueryHandler(
    IWorkoutsRepository repository,
    IValidator<GetExercisesByWorkoutIdQuery> validator)
    : IQueryHandler<GetExercisesByWorkoutIdQuery, Result<GetExercisesByWorkoutIdResponse, List<Error>>>
{
    public async Task<Result<GetExercisesByWorkoutIdResponse, List<Error>>> Handle(
        GetExercisesByWorkoutIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var workout = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<GetExercisesByWorkoutIdResponse, List<Error>>(
                WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        var exercises = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, cancellationToken);
        if (exercises is null)
        {
            return Result.Failure<GetExercisesByWorkoutIdResponse, List<Error>>(
                WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        var exerciseResponses = exercises.Select(MapExerciseToResponse).ToList();
        return Result.Success<GetExercisesByWorkoutIdResponse, List<Error>>(
            new GetExercisesByWorkoutIdResponse(exerciseResponses));
    }

    private static ExerciseDto MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new ExerciseDto(exercise.Name, setResponses);
    }
}
