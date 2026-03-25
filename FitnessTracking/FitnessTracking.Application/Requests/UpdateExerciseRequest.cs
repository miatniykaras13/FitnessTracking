using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record UpdateExerciseRequest(Guid WorkoutId, string ExerciseName, UpdateExerciseDto ExerciseDto);

