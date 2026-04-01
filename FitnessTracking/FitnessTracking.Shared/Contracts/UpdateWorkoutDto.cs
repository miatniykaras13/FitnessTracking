namespace FitnessTracking.Shared.Contracts;

public record UpdateWorkoutDto(
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);
