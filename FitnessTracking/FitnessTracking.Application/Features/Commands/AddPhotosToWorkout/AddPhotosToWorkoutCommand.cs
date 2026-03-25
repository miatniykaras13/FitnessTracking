using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public record AddPhotosToWorkoutCommand(
    Guid WorkoutId,
    string FileName,
    byte[] FileContent) : ICommand<Result<AddPhotosToWorkoutResponse, List<Error>>>;
