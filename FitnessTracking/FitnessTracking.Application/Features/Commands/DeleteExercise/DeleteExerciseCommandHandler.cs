using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteExercise;

public class DeleteExerciseCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<DeleteExerciseCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteExerciseAsync(request.WorkoutId, request.ExerciseName, cancellationToken);
    }
}
