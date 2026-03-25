using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateSet;

public record UpdateSetCommand(
    Guid WorkoutId,
    string ExerciseName,
    int SetIndex,
    UpdateSetDto SetDto) : ICommand<Result<UpdateSetResponse, Error>>;
