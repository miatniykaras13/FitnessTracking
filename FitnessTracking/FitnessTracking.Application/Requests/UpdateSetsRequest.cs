using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record UpdateSetsRequest(Guid WorkoutId, string ExerciseName, UpdateSetsDto SetDtos);
