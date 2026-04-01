using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateWorkout;

public record UpdateWorkoutCommand(
    Guid UserId,
    Guid WorkoutId,
    UpdateWorkoutDto WorkoutDto) : ICommand<Result<UpdateWorkoutResponse, List<Error>>>;
