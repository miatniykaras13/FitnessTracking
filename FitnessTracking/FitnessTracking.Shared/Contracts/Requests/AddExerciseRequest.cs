using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record AddExerciseRequest(Guid WorkoutId, AddExerciseDto ExerciseDto);

