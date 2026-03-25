using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public class AddExerciseCommandHandler(
    IWorkoutsRepository repository,
    IValidator<AddExerciseCommand> validator)
    : ICommandHandler<AddExerciseCommand, Result<AddExerciseResponse, List<Error>>>
{
    public async Task<Result<AddExerciseResponse, List<Error>>> Handle(AddExerciseCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var addResult = await repository.AddExerciseAsync(request.WorkoutId, exercise, cancellationToken);
        if (addResult.IsFailure)
        {
            return Result.Failure<AddExerciseResponse, List<Error>>(addResult.Error);
        }

        return Result.Success<AddExerciseResponse, List<Error>>(MapExerciseToResponse(exercise));
    }

    private static AddExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetDto(s.Reps, s.Weight))
            .ToList();

        return new AddExerciseResponse(exercise.Name, setResponses);
    }

    private static List<Set> MapSetDtos(IReadOnlyList<AddSetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }
}
