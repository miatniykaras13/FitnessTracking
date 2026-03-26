namespace FitnessTracking.Application.Features.Commands.CreateWorkout;

public record CreateWorkoutResponse(
    Guid WorkoutId,
    Guid UserId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate,
    DateTime CreatedAt);

