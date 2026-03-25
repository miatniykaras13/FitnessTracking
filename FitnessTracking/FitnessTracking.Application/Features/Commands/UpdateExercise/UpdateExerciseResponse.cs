using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public record UpdateExerciseResponse(string Name, IEnumerable<SetDto> Sets);

