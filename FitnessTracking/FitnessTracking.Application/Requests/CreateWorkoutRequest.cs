using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Requests;

public record CreateWorkoutRequest(
	Guid UserId,
	CreateWorkoutDto WorkoutDto);

