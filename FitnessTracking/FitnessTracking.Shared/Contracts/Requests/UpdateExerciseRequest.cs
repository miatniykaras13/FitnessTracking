using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateExerciseRequest(Guid WorkoutId, string ExerciseName, UpdateExerciseDto ExerciseDto);

