namespace FitnessTracking.Shared.Contracts;

public record ExerciseResponse(string Name, IEnumerable<SetResponse> Sets);

