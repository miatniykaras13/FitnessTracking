using System.Text.Json.Nodes;

namespace FitnessTracking.Application.Requests;

public record PatchSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex, JsonObject Patch);