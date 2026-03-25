using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkout;

public record DeleteWorkoutCommand(
    Guid WorkoutId,
    Guid UserId) : ICommand<UnitResult<List<Error>>>;
