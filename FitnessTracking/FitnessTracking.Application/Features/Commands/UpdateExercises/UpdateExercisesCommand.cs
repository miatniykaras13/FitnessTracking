using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public record UpdateExercisesCommand(
    Guid WorkoutId,
    UpdateExercisesDto ExerciseDtos) : ICommand<Result<UpdateExercisesResponse, Error>>;
