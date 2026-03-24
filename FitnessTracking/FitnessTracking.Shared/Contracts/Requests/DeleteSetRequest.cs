namespace FitnessTracking.Shared.Contracts.Requests;

public record DeleteSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex);

