using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Commands.PatchExercise;

public record PatchExerciseResponse(string Name, IEnumerable<SetDto> Sets);

