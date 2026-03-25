using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record UpdateSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex, UpdateSetDto SetDto);

