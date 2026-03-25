using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public class GetExercisesByWorkoutIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetExercisesByWorkoutIdQuery, Result<GetExercisesByWorkoutIdResponse, Error>>
{
    public async Task<Result<GetExercisesByWorkoutIdResponse, Error>> Handle(
        GetExercisesByWorkoutIdQuery request,
        CancellationToken cancellationToken)
    {
        var exercisesResult = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, cancellationToken);
        if (exercisesResult.IsFailure)
        {
            return Result.Failure<GetExercisesByWorkoutIdResponse, Error>(exercisesResult.Error);
        }

        var exerciseResponses = exercisesResult.Value.Select(MapExerciseToResponse).ToList();
        return Result.Success<GetExercisesByWorkoutIdResponse, Error>(new GetExercisesByWorkoutIdResponse(exerciseResponses));
    }

    private static ExerciseDto MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new ExerciseDto(exercise.Name, setResponses);
    }
}
