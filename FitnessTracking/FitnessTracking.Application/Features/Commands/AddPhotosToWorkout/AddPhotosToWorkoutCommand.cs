using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public record AddPhotosToWorkoutCommand(
    Guid WorkoutId) : ICommand<Result<AddPhotosToWorkoutResponse, Error>>;
