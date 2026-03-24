using System.Text.Json.Nodes;
using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record PatchExerciseRequest(Guid WorkoutId, string ExerciseName, JsonObject Patch);