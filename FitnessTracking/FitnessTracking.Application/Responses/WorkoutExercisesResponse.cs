namespace FitnessTracking.Application.Responses;

public record WorkoutExercisesResponse(IEnumerable<ExerciseResponse> Exercises);

