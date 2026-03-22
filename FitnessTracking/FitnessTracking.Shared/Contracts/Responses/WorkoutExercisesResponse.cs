namespace FitnessTracking.Shared.Contracts.Responses;

public record WorkoutExercisesResponse(IEnumerable<ExerciseResponse> Exercises);

