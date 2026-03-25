using System.Text.Json.Nodes;

namespace FitnessTracking.Application.Requests;

public record PatchWorkoutRequest(Guid WorkoutId, JsonObject Patch);