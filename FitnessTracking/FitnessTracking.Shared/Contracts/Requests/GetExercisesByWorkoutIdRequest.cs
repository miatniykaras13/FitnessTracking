namespace FitnessTracking.Shared.Contracts.Requests;

public record GetExercisesByWorkoutIdRequest(Guid WorkoutId);