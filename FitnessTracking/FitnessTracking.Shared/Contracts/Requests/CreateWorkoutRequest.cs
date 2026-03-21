namespace FitnessTracking.Shared.Contracts;

public record CreateWorkoutRequest(
	Guid UserId,
	string Title,
	string Type,
	TimeSpan Duration,
	int CaloriesBurned,
	DateTime WorkoutDate);

