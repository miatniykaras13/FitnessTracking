namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateWorkoutRequest(
    Guid WorkoutId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);
