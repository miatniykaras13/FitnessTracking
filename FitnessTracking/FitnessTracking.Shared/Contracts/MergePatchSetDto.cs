namespace FitnessTracking.Shared.Contracts;

public record MergePatchSetDto
{
    public int? Reps { get; set; }

    public double? Weight { get; set; }
}
