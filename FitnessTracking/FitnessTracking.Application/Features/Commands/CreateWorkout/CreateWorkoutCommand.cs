using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.CreateWorkout;

public record CreateWorkoutCommand(
    Guid UserId,
    CreateWorkoutDto WorkoutDto) : ICommand<Result<CreateWorkoutResponse, List<Error>>>;

