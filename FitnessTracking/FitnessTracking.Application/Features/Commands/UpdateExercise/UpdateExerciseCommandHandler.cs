using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateExerciseCommand> validator)
    : ICommandHandler<UpdateExerciseCommand, Result<UpdateExerciseResponse, List<Error>>>
{
    public async Task<Result<UpdateExerciseResponse, List<Error>>> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var updateResult = await repository.UpdateExerciseAsync(request.WorkoutId, request.ExerciseName, exercise, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateExerciseResponse, List<Error>>(updateResult.Error);
        }

        return Result.Success<UpdateExerciseResponse, List<Error>>(MapExerciseToResponse(exercise));
    }

    private static UpdateExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new UpdateExerciseResponse(exercise.Name, setResponses);
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

}
