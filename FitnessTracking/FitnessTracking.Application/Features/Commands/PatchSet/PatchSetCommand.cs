using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchSet;

public record PatchSetCommand(
    Guid WorkoutId,
    string ExerciseName,
    int SetIndex,
    JsonObject Patch) : ICommand<Result<PatchSetResponse, List<Error>>>;
