using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public record AddExerciseCommand(
    Guid WorkoutId,
    AddExerciseDto ExerciseDto) : ICommand<Result<AddExerciseResponse, List<Error>>>;
