using FitnessTracking.Domain.Abstractions;
using FitnessTracking.Domain.Enums;

namespace FitnessTracking.Domain.Models;

public class Workout : IDocument
{
    public required string Id { get; set; }

    public required string UserId { get; set; }

    public required string Title { get; set; }

    public WorkoutType Type { get; set; }

    public TimeSpan Duration { get; set; }

    public required int CaloriesBurned { get; set; }

    public DateTime WorkoutDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<Exercise> Exercises { get; set; } = [];

    public List<string> ProgressPhotos { get; set; } = [];
}