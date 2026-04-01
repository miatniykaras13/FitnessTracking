using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public class AddPhotosToWorkoutCommandHandler(
    ILocalFileStorage fileStorage,
    IWorkoutPhotosRepository photosRepository,
    IWorkoutsRepository workoutsRepository,
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

        var workoutResult = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(workoutResult.Error);
        }

        if (!string.Equals(workoutResult.Value.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var pathResult = await fileStorage.SaveWorkoutPhotoAsync(
            request.WorkoutId,
            request.FileName,
            request.FileContent,
            cancellationToken);
        if (pathResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(pathResult.Error);
        }

        var workout = workoutResult.Value;
        var photoId = Guid.NewGuid();
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            Path = pathResult.Value,
            WorkoutId = workout.Id,
            CreatedAt = DateTime.UtcNow
        };

        var addPhotoResult = await photosRepository.AddAsync(photo, cancellationToken);
        if (addPhotoResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(addPhotoResult.Error);
        }

        workout.ProgressPhotos.Add(photo);

        var isWorkoutUpdated = await workoutsRepository.UpdateAsync(workout, cancellationToken);
        if (!isWorkoutUpdated)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        return new AddPhotosToWorkoutResponse(photoId, photo.Path);
    }
}
