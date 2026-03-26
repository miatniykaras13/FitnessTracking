namespace FitnessTracking.Application.Filters;

public record WorkoutFilter(
    string? Type,
    DateTime? WorkoutDateFrom,
    DateTime? WorkoutDateTo,
    TimeSpan? DurationFrom,
    TimeSpan? DurationTo);