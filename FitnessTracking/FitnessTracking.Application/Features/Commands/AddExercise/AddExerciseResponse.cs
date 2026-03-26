using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public record AddExerciseResponse(string Name, IEnumerable<SetDto> Sets);

