using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public record UpdateSetsCommand(
    Guid UserId,
    Guid WorkoutId,
    string ExerciseName,
    UpdateSetsDto SetDtos) : ICommand<Result<UpdateSetsResponse, List<Error>>>;
