using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public class GetExercisesByWorkoutIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetExercisesByWorkoutIdQuery, Result<WorkoutExercisesResponse, Error>>
{
    public async Task<Result<WorkoutExercisesResponse, Error>> Handle(
        GetExercisesByWorkoutIdQuery request,
        CancellationToken cancellationToken)
    {
        var exercisesResult = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, cancellationToken);
        if (exercisesResult.IsFailure)
        {
            return Result.Failure<WorkoutExercisesResponse, Error>(exercisesResult.Error);
        }

        var exerciseResponses = exercisesResult.Value.Select(MapExerciseToResponse).ToList();
        return Result.Success<WorkoutExercisesResponse, Error>(new WorkoutExercisesResponse(exerciseResponses));
    }

    private static ExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return new ExerciseResponse(exercise.Name, setResponses);
    }
}
