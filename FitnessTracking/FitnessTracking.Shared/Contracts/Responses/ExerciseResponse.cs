namespace FitnessTracking.Shared.Contracts.Responses;

public record ExerciseResponse(string Name, IEnumerable<SetResponse> Sets);

