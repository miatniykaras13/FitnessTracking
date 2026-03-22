namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateWorkoutRequest(
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate);
