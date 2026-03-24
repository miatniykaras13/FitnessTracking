using System.Text.Json.Nodes;

namespace FitnessTracking.Shared.Contracts.Requests;

public record PatchWorkoutRequest(Guid WorkoutId, JsonObject Patch);