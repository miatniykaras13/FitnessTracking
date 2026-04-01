namespace FitnessTracking.Shared.Contracts;

public record ExerciseDto(string Name, IReadOnlyList<SetDto> Sets);
