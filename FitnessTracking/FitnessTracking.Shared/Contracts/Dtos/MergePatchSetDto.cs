namespace FitnessTracking.Shared.Contracts.Dtos;

public record MergePatchSetDto
{
    public int? Reps { get; set; }

    public double? Weight { get; set; }
}