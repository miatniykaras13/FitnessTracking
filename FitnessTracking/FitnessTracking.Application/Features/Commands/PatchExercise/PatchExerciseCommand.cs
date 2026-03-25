using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.PatchExercise;

public record PatchExerciseCommand(
    Guid WorkoutId,
    string ExerciseName,
    JsonObject Patch) : ICommand<Result<ExerciseResponse, Error>>;
