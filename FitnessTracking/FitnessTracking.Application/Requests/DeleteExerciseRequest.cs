namespace FitnessTracking.Application.Requests;

public record DeleteExerciseRequest(Guid WorkoutId, string ExerciseName);

