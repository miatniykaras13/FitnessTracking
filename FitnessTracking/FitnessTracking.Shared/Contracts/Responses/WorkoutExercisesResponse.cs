namespace FitnessTracking.Shared.Contracts.Responses;

public record WorkoutExercisesResponse(Guid WorkoutId, IEnumerable<ExerciseResponse> Exercises);

