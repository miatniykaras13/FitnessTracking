using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record AddExerciseRequest(Guid WorkoutId, AddExerciseDto ExerciseDto);

