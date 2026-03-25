using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;

public record DeleteWorkoutPhotoCommand(
    Guid WorkoutId,
    Guid PhotoId) : ICommand<UnitResult<List<Error>>>;

