namespace FitnessTracking.Application.Responses;

public record WorkoutListResponse(IEnumerable<WorkoutResponse> Workouts, int Total);

