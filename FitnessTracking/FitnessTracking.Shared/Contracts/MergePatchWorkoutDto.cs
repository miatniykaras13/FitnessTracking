namespace FitnessTracking.Shared.Contracts;

public record MergePatchWorkoutDto
{
    public string? Title { get; set; }
    
    public string? Type { get; set; }

    public TimeSpan? Duration { get; set; }
    
    public int? CaloriesBurned { get; set; }

    public DateTime? WorkoutDate { get; set; }
}