namespace FitnessTracking.Shared.Errors;

public static class WorkoutErrors
{
    public static Error NotFound(Guid workoutId) =>
        Error.NotFound("workout", $"Workout with id {workoutId} was not found.");

    public static Error InvalidWorkoutType(string type) =>
        Error.Validation("workout.type", $"{type} is not a valid workout type.");
}

