using FitnessTracking.Shared.Contracts.Dtos;

namespace FitnessTracking.Shared.Contracts.Requests;

public record UpdateWorkoutRequest(
    Guid WorkoutId,
    Guid UserId,
    UpdateWorkoutDto WorkoutDto);
