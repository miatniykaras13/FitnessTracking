namespace FitnessTracking.Application.Features.Commands.UpdateWorkout;

public record UpdateWorkoutResponse(
    Guid WorkoutId,
    Guid UserId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate,
    DateTime CreatedAt);

