using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteExercise;

public record DeleteExerciseCommand(
    Guid WorkoutId,
    string ExerciseName) : ICommand<UnitResult<List<Error>>>;
