using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public record GetExercisesByWorkoutIdQuery(Guid WorkoutId) : IQuery<Result<WorkoutExercisesResponse, Error>>;
