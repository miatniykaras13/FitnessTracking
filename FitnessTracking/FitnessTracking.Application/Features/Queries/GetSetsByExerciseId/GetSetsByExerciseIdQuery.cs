using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetSetsByExerciseId;

public record GetSetsByExerciseIdQuery(
    Guid WorkoutId,
    string ExerciseName) : IQuery<Result<SetListResponse, Error>>;
