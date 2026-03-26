namespace FitnessTracking.Shared.Errors;

public static class WorkoutErrors
{
    public static Error WorkoutNotFound(Guid workoutId) =>
        Error.NotFound("workout", $"Workout with id {workoutId} was not found.");

    public static Error WorkoutPhotoNotFound(Guid workoutId, Guid photoId) =>
        Error.NotFound("workout.photo", $"Photo with id {photoId} was not found in workout with id {workoutId}.");

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

    public static Error WorkoutAccessDenied(Guid workoutId, Guid userId) =>
        Error.Forbidden(
            "workout",
            $"User with id {userId} has no access to workout with id {workoutId}.");
}