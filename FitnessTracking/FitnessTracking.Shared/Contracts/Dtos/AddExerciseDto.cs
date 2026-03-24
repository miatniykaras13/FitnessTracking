namespace FitnessTracking.Shared.Contracts.Dtos;

public record AddExerciseDto(string Name, IReadOnlyList<SetDto> Sets);

