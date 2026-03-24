using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateExercisesRequest(Guid WorkoutId, UpdateExercisesDto ExerciseDtos);

