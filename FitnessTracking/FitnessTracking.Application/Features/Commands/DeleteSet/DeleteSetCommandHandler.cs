using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.DeleteSet;

public class DeleteSetCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<DeleteSetCommand, UnitResult<Error>>
{
    public async Task<UnitResult<Error>> Handle(DeleteSetCommand request, CancellationToken cancellationToken)
    {
        if (request.SetIndex < 0)
        {
            return UnitResult.Failure(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        return await repository.DeleteSetAsync(request.WorkoutId, request.ExerciseName, request.SetIndex, cancellationToken);
    }
}
