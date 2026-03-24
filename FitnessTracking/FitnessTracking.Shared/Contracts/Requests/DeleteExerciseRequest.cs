namespace FitnessTracking.Shared.Contracts.Requests;

public record DeleteExerciseRequest(Guid WorkoutId, string ExerciseName);

