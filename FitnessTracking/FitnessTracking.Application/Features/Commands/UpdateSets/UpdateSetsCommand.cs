using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public record UpdateSetsCommand(
    Guid WorkoutId,
    string ExerciseName,
    UpdateSetsDto SetDtos) : ICommand<Result<SetListResponse, Error>>;
