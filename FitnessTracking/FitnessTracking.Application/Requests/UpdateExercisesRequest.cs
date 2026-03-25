using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record UpdateExercisesRequest(Guid WorkoutId, UpdateExercisesDto ExerciseDtos);

