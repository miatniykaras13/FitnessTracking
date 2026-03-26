namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public record PatchWorkoutResponse(
    Guid WorkoutId,
    Guid UserId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate,
    DateTime CreatedAt);

