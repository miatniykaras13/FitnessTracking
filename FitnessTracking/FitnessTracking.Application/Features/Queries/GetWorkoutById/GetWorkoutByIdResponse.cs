namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public record GetWorkoutByIdResponse(
    Guid WorkoutId,
    Guid UserId,
    string Title,
    string Type,
    TimeSpan Duration,
    int CaloriesBurned,
    DateTime WorkoutDate,
    DateTime CreatedAt,
    IEnumerable<string> ProgressPhotos);

