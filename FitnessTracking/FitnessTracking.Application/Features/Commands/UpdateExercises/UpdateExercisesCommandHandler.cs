using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public class UpdateExercisesCommandHandler(
    IWorkoutsRepository repository,
    IValidator<UpdateExercisesCommand> validator)
    : ICommandHandler<UpdateExercisesCommand, Result<UpdateExercisesResponse, List<Error>>>
{
    public async Task<Result<UpdateExercisesResponse, List<Error>>> Handle(
        UpdateExercisesCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Exercise).ToLower());
        }

        var exercises = request.ExerciseDtos.Exercises.Select(MapExerciseDtoToDomain).ToList();

        var updateResult = await repository.UpdateExercisesAsync(request.WorkoutId, exercises, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateExercisesResponse, List<Error>>(updateResult.Error);
        }

        var response = exercises.Select(MapExerciseToResponse).ToList();
        return Result.Success<UpdateExercisesResponse, List<Error>>(new UpdateExercisesResponse(response));
    }

    private static Exercise MapExerciseDtoToDomain(UpdateExerciseDto dto)
    {
        return new Exercise
        {
            Name = dto.Name,
            Sets = MapSetDtos(dto.Sets)
        };
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

    private static ExerciseDto MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new ExerciseDto(exercise.Name, setResponses);
    }

}
