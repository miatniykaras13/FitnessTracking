namespace FitnessTracking.Domain.Models;

public class Exercise
{
    public required string Name { get; set; }

    public List<Set> Sets { get; set; } = [];
}
