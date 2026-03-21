using FitnessTracking.Application.Abstractions;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;

namespace FitnessTracking.Application.Services;

public class WorkoutsService(IWorkoutsRepository repository)
{
    public async Task<WorkoutResponse> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct)
    {
        var workout = await repository.GetByIdAsync(request.WorkoutId, ct);
        return MapToResponse(workout);
    }

    public async Task<CreateWorkoutResponse> AddAsync(CreateWorkoutRequest request, CancellationToken ct)
    {
        var workoutId = Guid.NewGuid();
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = request.UserId.ToString(),
            Title = request.Title,
            Type = Enum.Parse<WorkoutType>(request.Type),
            Duration = request.Duration,
            CaloriesBurned = request.CaloriesBurned,
            WorkoutDate = request.WorkoutDate,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(workout, ct);

        return new CreateWorkoutResponse(workoutId);
    }

    public async Task<UpdateWorkoutResponse> UpdateAsync(UpdateWorkoutRequest request, CancellationToken ct)
    {
        var workout = await repository.GetByIdAsync(request.WorkoutId, ct);

        workout.Title = request.Title;
        workout.Type = Enum.Parse<WorkoutType>(request.Type);
        workout.Duration = request.Duration;
        workout.CaloriesBurned = request.CaloriesBurned;
        workout.WorkoutDate = request.WorkoutDate;

        await repository.UpdateAsync(workout, ct);

        return new UpdateWorkoutResponse(request.WorkoutId);
    }

    public async Task DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct)
    {
        await repository.DeleteAsync(request.WorkoutId, ct);
    }

    public async Task<WorkoutListResponse> GetByUserIdAsync(GetWorkoutsByUserIdRequest request, CancellationToken ct)
    {
        var workouts = await repository.GetByUserIdAsync(request.UserId, ct);
        var responses = workouts.Select(MapToResponse).ToList();
        return new WorkoutListResponse(responses);
    }

    public async Task<WorkoutExercisesResponse> GetExercisesByWorkoutIdAsync(GetWorkoutByIdRequest request, CancellationToken ct)
    {
        var exercises = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, ct);
        var exerciseResponses = exercises.Select(MapExerciseToResponse).ToList();
        return new WorkoutExercisesResponse(request.WorkoutId, exerciseResponses);
    }

    public Task AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken ct)
    {
        return repository.AddPhotosToWorkoutAsync(workoutId, ct);
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