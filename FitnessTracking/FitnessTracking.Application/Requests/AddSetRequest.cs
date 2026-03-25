using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record AddSetRequest(Guid WorkoutId, string ExerciseName, AddSetDto SetDto);

