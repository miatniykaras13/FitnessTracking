using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public record UpdateExercisesResponse(IEnumerable<ExerciseDto> Exercises);

