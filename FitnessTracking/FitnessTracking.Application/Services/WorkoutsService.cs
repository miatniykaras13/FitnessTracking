using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Services;

public class WorkoutsService(IWorkoutsRepository repository) : IWorkoutsService
{
    public async Task<Result<WorkoutResponse, Error>> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);

        if (workoutResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutResult.Error);
        }

        return Result.Success<WorkoutResponse, Error>(MapToResponse(workoutResult.Value));
    }

    public async Task<Result<CreateWorkoutResponse, Error>> AddAsync(Guid userId, CreateWorkoutRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<WorkoutType>(request.Type, true, out var workoutType))
        {
            return Result.Failure<CreateWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.Type));
        }

        var workoutId = Guid.NewGuid();
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = request.Title,
            Type = workoutType,
            Duration = request.Duration,
            CaloriesBurned = request.CaloriesBurned,
            WorkoutDate = request.WorkoutDate,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await repository.AddAsync(workout, ct);

        if (addResult.IsFailure)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(addResult.Error);
        }

        return Result.Success<CreateWorkoutResponse, Error>(new CreateWorkoutResponse(workoutId));
    }

    public async Task<Result<UpdateWorkoutResponse, Error>> UpdateAsync(Guid workoutId, UpdateWorkoutRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<WorkoutType>(request.Type, true, out var workoutType))
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.Type));
        }

        var workoutResult = await repository.GetByIdAsync(workoutId, ct);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;

        workout.Title = request.Title;
        workout.Type = workoutType;
        workout.Duration = request.Duration;
        workout.CaloriesBurned = request.CaloriesBurned;
        workout.WorkoutDate = request.WorkoutDate;

        var updateResult = await repository.UpdateAsync(workout, ct);

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateWorkoutResponse, Error>(updateResult.Error);
        }

        return Result.Success<UpdateWorkoutResponse, Error>(new UpdateWorkoutResponse(workoutId));
    }

    public async Task<UnitResult<Error>> DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct)
    {
        return await repository.DeleteAsync(request.WorkoutId, ct);
    }

    public async Task<Result<WorkoutListResponse, Error>> GetByUserIdAsync(GetWorkoutsByUserIdRequest request, CancellationToken ct)
    {
        var workoutsResult = await repository.GetByUserIdAsync(request.UserId, ct);
        if (workoutsResult.IsFailure)
        {
            return Result.Failure<WorkoutListResponse, Error>(workoutsResult.Error);
        }

        var responses = workoutsResult.Value.Select(MapToResponse).ToList();
        return Result.Success<WorkoutListResponse, Error>(new WorkoutListResponse(responses));
    }

    public async Task<Result<WorkoutExercisesResponse, Error>> GetExercisesByWorkoutIdAsync(
        GetExercisesByWorkoutIdRequest request,
        CancellationToken ct)
    {
        var exercisesResult = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, ct);
        if (exercisesResult.IsFailure)
        {
            return Result.Failure<WorkoutExercisesResponse, Error>(exercisesResult.Error);
        }

        var exerciseResponses = exercisesResult.Value.Select(MapExerciseToResponse).ToList();
        return Result.Success<WorkoutExercisesResponse, Error>(new WorkoutExercisesResponse(exerciseResponses));
    }

    public async Task<UnitResult<Error>> AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken ct)
    {
        return await repository.AddPhotosToWorkoutAsync(workoutId, ct);
    }

    private static WorkoutResponse MapToResponse(Workout workout)
    {
        return new WorkoutResponse(
            WorkoutId: Guid.Parse(workout.Id),
            UserId: Guid.Parse(workout.UserId),
            Title: workout.Title,
            Type: workout.Type.ToString(),
            Duration: workout.Duration,
            CaloriesBurned: workout.CaloriesBurned,
            WorkoutDate: workout.WorkoutDate,
            CreatedAt: workout.CreatedAt);
    }

    private static ExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return new ExerciseResponse(exercise.Name, setResponses);
    }
}