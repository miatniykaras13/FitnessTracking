using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public record PatchWorkoutCommand(
    Guid WorkoutId,
    JsonObject Patch) : ICommand<Result<WorkoutResponse, Error>>;
