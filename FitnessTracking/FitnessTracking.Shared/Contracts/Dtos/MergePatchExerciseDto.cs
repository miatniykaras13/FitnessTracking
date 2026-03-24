namespace FitnessTracking.Shared.Contracts.Dtos;

public record MergePatchExerciseDto
{
    public string? Name { get; set; }

    public IReadOnlyList<AddSetDto>? Sets { get; set; }
}