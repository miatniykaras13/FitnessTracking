namespace FitnessTracking.Shared.Contracts;

public record CreateWorkoutDto(
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);