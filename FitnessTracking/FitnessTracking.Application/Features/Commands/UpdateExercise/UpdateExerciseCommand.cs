using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public record UpdateExerciseCommand(
    Guid WorkoutId,
    string ExerciseName,
    UpdateExerciseDto ExerciseDto) : ICommand<Result<UpdateExerciseResponse, Error>>;
