using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public record GetExercisesByWorkoutIdResponse(IEnumerable<ExerciseDto> Exercises);

