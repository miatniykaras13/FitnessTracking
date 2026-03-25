using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkout;

public class DeleteWorkoutCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<DeleteWorkoutCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(request.WorkoutId, cancellationToken);
    }
}
