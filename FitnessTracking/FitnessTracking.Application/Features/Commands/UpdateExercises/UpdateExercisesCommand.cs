using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public record UpdateExercisesCommand(
    Guid UserId,
    Guid WorkoutId,
    UpdateExercisesDto ExerciseDtos) : ICommand<Result<UpdateExercisesResponse, List<Error>>>;
