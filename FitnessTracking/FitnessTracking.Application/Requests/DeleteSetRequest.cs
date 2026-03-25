namespace FitnessTracking.Application.Requests;

public record DeleteSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex);

