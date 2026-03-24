using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record AddSetRequest(Guid WorkoutId, string ExerciseName, AddSetDto SetDto);

