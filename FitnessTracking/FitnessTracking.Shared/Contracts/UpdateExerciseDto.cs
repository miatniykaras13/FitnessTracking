namespace FitnessTracking.Shared.Contracts;

public record UpdateExerciseDto(string Name, IReadOnlyList<SetDto> Sets);

