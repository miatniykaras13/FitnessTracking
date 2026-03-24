namespace FitnessTracking.Shared.Contracts.Dtos;

public record UpdateExerciseDto(string Name, IReadOnlyList<SetDto> Sets);

