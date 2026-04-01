using FitnessTracking.Domain.Abstractions;

namespace FitnessTracking.Domain.Models;

public class WorkoutPhoto : IDocument
{
    public required string Id { get; set; }

    public required string Path { get; set; }

    public required string WorkoutId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Workout? Workout { get; set; }
}