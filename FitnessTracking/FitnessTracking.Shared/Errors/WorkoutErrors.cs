namespace FitnessTracking.Shared.Errors;

public static class WorkoutErrors
{
    public static Error WorkoutNotFound(Guid workoutId) =>
        Error.NotFound("workout", $"Workout with id {workoutId} was not found.");
    
    public static Error TitleRequired() =>
        Error.Validation("workout.title", "Workout title is required.");

    public static Error TypeRequired() =>
        Error.Validation("workout.type", "Workout type is required.");
    
    public static Error DurationMustBePositive() =>
        Error.Validation("workout.duration", "Workout duration must be greater than zero.");

    public static Error CaloriesBurnedMustBeNonNegative() =>
        Error.Validation("workout.caloriesBurned", "Calories burned must be greater than or equal to zero.");

    public static Error WorkoutDateRequired() =>
        Error.Validation("workout.workoutDate", "Workout date is required.");

    public static Error UserIdRequired() =>
        Error.Validation("workout.userId", "Workout user id is required.");

    public static Error ExerciseNameRequired() =>
        Error.Validation("workout.exercise.name", "Exercise name is required.");

    public static Error ExerciseSetsRequired() =>
        Error.Validation("workout.exercise.sets", "Exercise sets are required.");

    public static Error SetRepsMustBePositive() =>
        Error.Validation("workout.exercise.set.reps", "Set reps must be greater than zero.");

    public static Error SetWeightMustBeNonNegative() =>
        Error.Validation("workout.exercise.set.weight", "Set weight must be greater than or equal to zero.");

    public static Error ExerciseNotFound(Guid workoutId, string exerciseName) =>
        Error.NotFound(
            "workout.exercise",
            $"Exercise with name {exerciseName} was not found in workout with id {workoutId}.");

    public static Error ExerciseAlreadyExists(Guid workoutId, string exerciseName) =>
        Error.Conflict(
            "workout.exercise",
            $"Exercise with name {exerciseName} already exists in workout with id {workoutId}.");

    public static Error SetNotFound(Guid workoutId, string exerciseName, int setIndex) =>
        Error.NotFound(
            "workout.exercise.set",
            $"Set with index {setIndex} was not found in exercise {exerciseName} in workout with id {workoutId}.");

    public static Error InvalidSetIndex(int setIndex) =>
        Error.Validation("workout.exercise.set.index", $"Set index {setIndex} is invalid.");

    public static Error InvalidWorkoutType(string? type) =>
        Error.Validation("workout.type", $"{type} is not a valid workout type.");
}