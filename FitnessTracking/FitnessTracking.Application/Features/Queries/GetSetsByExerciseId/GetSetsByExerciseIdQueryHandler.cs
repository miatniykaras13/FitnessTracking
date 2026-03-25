using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetSetsByExerciseId;

public class GetSetsByExerciseIdQueryHandler(IWorkoutsRepository repository)
    : IQueryHandler<GetSetsByExerciseIdQuery, Result<SetListResponse, Error>>
{
    public async Task<Result<SetListResponse, Error>> Handle(
        GetSetsByExerciseIdQuery request,
        CancellationToken cancellationToken)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<SetListResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        
        if (exercise is null)
        {
            return Result.Failure<SetListResponse, Error>(
                WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return Result.Success<SetListResponse, Error>(new SetListResponse(setResponses));
    }
}
