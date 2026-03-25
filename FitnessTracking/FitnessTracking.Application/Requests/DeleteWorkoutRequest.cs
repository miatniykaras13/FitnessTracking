namespace FitnessTracking.Application.Requests;

public record DeleteWorkoutRequest(Guid WorkoutId, Guid UserId);
