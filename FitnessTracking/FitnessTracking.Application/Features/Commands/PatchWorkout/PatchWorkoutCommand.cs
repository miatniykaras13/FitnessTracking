using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public record PatchWorkoutCommand(
    Guid UserId,
    Guid WorkoutId,
    JsonObject Patch) : ICommand<Result<PatchWorkoutResponse, List<Error>>>;
