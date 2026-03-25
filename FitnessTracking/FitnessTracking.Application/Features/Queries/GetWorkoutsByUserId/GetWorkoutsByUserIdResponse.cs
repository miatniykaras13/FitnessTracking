using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public record GetWorkoutsByUserIdResponse(IEnumerable<WorkoutNoUserIdDto> Workouts, int Total);

