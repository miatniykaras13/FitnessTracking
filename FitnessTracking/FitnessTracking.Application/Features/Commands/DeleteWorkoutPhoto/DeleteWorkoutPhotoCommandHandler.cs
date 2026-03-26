using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;

public class DeleteWorkoutPhotoCommandHandler(
    IWorkoutPhotosRepository photosRepository,
    IWorkoutsRepository workoutsRepository,
    ILocalFileStorage fileStorage,
    IValidator<DeleteWorkoutPhotoCommand> validator)
    : ICommandHandler<DeleteWorkoutPhotoCommand, UnitResult<List<Error>>>
{
    public async Task<UnitResult<List<Error>>> Handle(DeleteWorkoutPhotoCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UnitResult.Failure(validationResult.Errors.ToErrors(nameof(WorkoutPhoto).ToLower()));
        }

        var workoutResult = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workoutResult.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(workoutResult.Error);
        }

        if (!string.Equals(workoutResult.Value.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return UnitResult.Failure<List<Error>>(WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var photoResult = await photosRepository.GetByWorkoutIdAndPhotoIdAsync(
            request.WorkoutId,
            request.PhotoId,
            cancellationToken);
        if (photoResult.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(photoResult.Error);
        }

        var photo = photoResult.Value;

        var deleteDbResult = await photosRepository.DeleteByWorkoutIdAndPhotoIdAsync(
            request.WorkoutId,
            request.PhotoId,
            cancellationToken);
        if (deleteDbResult.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(deleteDbResult.Error);
        }

        var ownedWorkoutResult = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (ownedWorkoutResult.IsSuccess)
        {
            var workout = ownedWorkoutResult.Value;
            workout.ProgressPhotos.RemoveAll(p => p.Id.Equals(request.PhotoId.ToString()));
            var updateResult = await workoutsRepository.UpdateAsync(workout, cancellationToken);
            if (updateResult.IsFailure)
            {
                return UnitResult.Failure<List<Error>>(updateResult.Error);
            }
        }

        var deleteFileResult = await fileStorage.DeleteWorkoutPhotoAsync(photo.Path, cancellationToken);
        if (deleteFileResult.IsFailure)
        {
            return UnitResult.Failure<List<Error>>(deleteFileResult.Error);
        }

        return UnitResult.Success<List<Error>>();
    }
}

