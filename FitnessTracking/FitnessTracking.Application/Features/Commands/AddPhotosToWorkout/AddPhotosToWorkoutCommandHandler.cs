using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public class AddPhotosToWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IValidator<AddPhotosToWorkoutCommand> validator)
    : ICommandHandler<AddPhotosToWorkoutCommand, Result<AddPhotosToWorkoutResponse, List<Error>>>
{
    public async Task<Result<AddPhotosToWorkoutResponse, List<Error>>> Handle(AddPhotosToWorkoutCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var photoIdResult = await repository.AddPhotosToWorkoutAsync(request.WorkoutId, cancellationToken);

        if (photoIdResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(photoIdResult.Error);
        }

        return new AddPhotosToWorkoutResponse(photoIdResult.Value);
    }
}
