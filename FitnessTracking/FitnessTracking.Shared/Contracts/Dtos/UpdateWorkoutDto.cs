namespace FitnessTracking.Shared.Contracts.Dtos;

public record UpdateWorkoutDto(
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);