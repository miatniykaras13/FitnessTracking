using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public class AddPhotosToWorkoutCommandHandler(IWorkoutsRepository repository)
    : ICommandHandler<AddPhotosToWorkoutCommand, Result<AddPhotosToWorkoutResponse, Error>>
{
    public async Task<Result<AddPhotosToWorkoutResponse, Error>> Handle(AddPhotosToWorkoutCommand request, CancellationToken cancellationToken)
    {
        var photoIdResult = await repository.AddPhotosToWorkoutAsync(request.WorkoutId, cancellationToken);

        if (photoIdResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, Error>(photoIdResult.Error);
        }

        return new AddPhotosToWorkoutResponse(photoIdResult.Value);
    }
}
