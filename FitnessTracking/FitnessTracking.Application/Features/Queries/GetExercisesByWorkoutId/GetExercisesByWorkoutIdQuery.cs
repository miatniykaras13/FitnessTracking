using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public record GetExercisesByWorkoutIdQuery(
	Guid WorkoutId) : IQuery<Result<GetExercisesByWorkoutIdResponse, List<Error>>>;
