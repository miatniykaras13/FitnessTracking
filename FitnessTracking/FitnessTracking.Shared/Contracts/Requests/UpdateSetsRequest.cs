using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateSetsRequest(Guid WorkoutId, string ExerciseName, UpdateSetsDto SetDtos);
