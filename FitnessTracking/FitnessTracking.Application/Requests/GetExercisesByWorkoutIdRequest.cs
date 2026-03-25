namespace FitnessTracking.Application.Requests;

public record GetExercisesByWorkoutIdRequest(Guid WorkoutId);