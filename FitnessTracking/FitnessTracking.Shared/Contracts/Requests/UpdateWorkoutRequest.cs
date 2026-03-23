namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateWorkoutRequest(
    Guid WorkoutId,
    Guid UserId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);
