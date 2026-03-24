using System.Text.Json.Nodes;
using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record PatchSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex, JsonObject Patch);