namespace FitnessTracking.Application.Requests;

public record AddPhotosToWorkoutRequest(Guid WorkoutId, Guid UserId);