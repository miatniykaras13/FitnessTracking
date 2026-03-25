using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;

public record GetWorkoutPhotoQuery(
    Guid WorkoutId,
    Guid PhotoId) : IQuery<Result<GetWorkoutPhotoResponse, List<Error>>>;

