namespace FitnessTracking.Shared.Contracts.Requests;

public record CreateWorkoutRequest(
	Guid UserId,
	string Title,
	string Type,
	TimeSpan Duration,
	int CaloriesBurned,
	DateTime WorkoutDate);

