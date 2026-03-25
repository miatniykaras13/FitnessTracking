namespace FitnessTracking.Application.Responses;

public record ExerciseResponse(string Name, IEnumerable<SetResponse> Sets);

