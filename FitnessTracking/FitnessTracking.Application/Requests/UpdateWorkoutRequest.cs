using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record UpdateWorkoutRequest(
    Guid WorkoutId,
    Guid UserId,
    UpdateWorkoutDto WorkoutDto);
