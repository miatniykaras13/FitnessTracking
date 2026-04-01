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
    private const string PhotoExtensionErrorCode = "photo.extension";
    private const string AllowedExtensionsMessage = "Only jpg, jpeg and png are allowed.";

    public async Task<Result<AddPhotosToWorkoutResponse, List<Error>>> Handle(AddPhotosToWorkoutCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        var workout = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var photoPath = await fileStorage.SaveWorkoutPhotoAsync(
            request.WorkoutId,
            request.FileName,
            request.FileContent,
            cancellationToken);
        if (photoPath is null)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(
                Error.Validation(PhotoExtensionErrorCode, AllowedExtensionsMessage));
        }

        var photoId = Guid.NewGuid();
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            Path = photoPath,
            WorkoutId = workout.Id,
            CreatedAt = DateTime.UtcNow
        };

        var addedPhoto = await photosRepository.AddAsync(photo, cancellationToken);

        workout.ProgressPhotos.Add(addedPhoto);

        var isWorkoutUpdated = await workoutsRepository.UpdateAsync(workout, cancellationToken);
        if (!isWorkoutUpdated)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, List<Error>>(WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        return new AddPhotosToWorkoutResponse(Guid.Parse(addedPhoto.Id), addedPhoto.Path);
    }
}
