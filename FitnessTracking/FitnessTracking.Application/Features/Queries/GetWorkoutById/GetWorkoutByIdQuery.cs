using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public record GetWorkoutByIdQuery(Guid WorkoutId) : IQuery<Result<WorkoutResponse, Error>>;
