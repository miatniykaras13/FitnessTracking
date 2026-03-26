using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddSet;

public record AddSetCommand(
    Guid WorkoutId,
    Guid UserId,
    string ExerciseName,
    AddSetDto SetDto) : ICommand<Result<AddSetResponse, List<Error>>>;
