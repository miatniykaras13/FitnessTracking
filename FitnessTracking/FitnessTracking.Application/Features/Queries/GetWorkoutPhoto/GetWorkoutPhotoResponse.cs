namespace FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;

public record GetWorkoutPhotoResponse(
    Guid PhotoId,
    Guid WorkoutId,
    string Path,
    DateTime CreatedAt);

