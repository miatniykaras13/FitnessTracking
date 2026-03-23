namespace FitnessTracking.Shared.Contracts.Requests;

public record AddPhotosToWorkoutRequest(Guid WorkoutId, Guid UserId);