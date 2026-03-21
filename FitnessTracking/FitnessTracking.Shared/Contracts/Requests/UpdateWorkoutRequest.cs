namespace FitnessTracking.Shared.Contracts;

public record UpdateWorkoutRequest(
    Guid WorkoutId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);
