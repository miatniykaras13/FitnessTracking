using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteSet;

public record DeleteSetCommand(
    Guid UserId,
    Guid WorkoutId,
    string ExerciseName,
    int SetIndex) : ICommand<UnitResult<List<Error>>>;
