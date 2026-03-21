namespace FitnessTracking.Shared.Contracts;

public record WorkoutListResponse(IEnumerable<WorkoutResponse> Workouts);

