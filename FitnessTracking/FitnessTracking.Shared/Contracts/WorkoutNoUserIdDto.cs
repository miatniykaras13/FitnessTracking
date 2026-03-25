namespace FitnessTracking.Shared.Contracts;

public record WorkoutNoUserIdDto(
    Guid WorkoutId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate,
    DateTime CreatedAt);