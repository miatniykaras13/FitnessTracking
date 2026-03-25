namespace FitnessTracking.Shared.Contracts;

public record AddExerciseDto(string Name, IReadOnlyList<AddSetDto> Sets);

