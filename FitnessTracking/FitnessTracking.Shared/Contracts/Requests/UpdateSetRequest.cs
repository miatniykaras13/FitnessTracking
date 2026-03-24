using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateSetRequest(Guid WorkoutId, string ExerciseName, int SetIndex, UpdateSetDto SetDto);

