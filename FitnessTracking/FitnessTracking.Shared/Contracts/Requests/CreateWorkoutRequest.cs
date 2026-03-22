namespace FitnessTracking.Shared.Contracts.Requests;

public record CreateWorkoutRequest(
	string Title,
	string Type,
	TimeSpan Duration,
	int CaloriesBurned,
	DateTime WorkoutDate);

