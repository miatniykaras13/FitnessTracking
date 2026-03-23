namespace FitnessTracking.Shared.Contracts.Dtos;

public record CreateWorkoutDto(
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);