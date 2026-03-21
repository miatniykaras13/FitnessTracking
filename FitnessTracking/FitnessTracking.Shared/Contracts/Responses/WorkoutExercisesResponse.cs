namespace FitnessTracking.Shared.Contracts;

public record WorkoutExercisesResponse(Guid WorkoutId, IEnumerable<ExerciseResponse> Exercises);

