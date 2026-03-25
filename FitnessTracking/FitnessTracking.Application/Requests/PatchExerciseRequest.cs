using System.Text.Json.Nodes;

namespace FitnessTracking.Application.Requests;

public record PatchExerciseRequest(Guid WorkoutId, string ExerciseName, JsonObject Patch);