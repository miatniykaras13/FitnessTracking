namespace FitnessTracking.Shared.Contracts.Responses;

public record WorkoutListResponse(IEnumerable<WorkoutResponse> Workouts, int Total);

