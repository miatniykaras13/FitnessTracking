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

        var workout = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return UnitResult.Failure<List<Error>>(
                WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }

        if (!string.Equals(workout.UserId, request.UserId.ToString(), StringComparison.Ordinal))
        {
            return UnitResult.Failure<List<Error>>(
                WorkoutErrors.WorkoutAccessDenied(request.WorkoutId, request.UserId));
        }

        var photo = await photosRepository.GetByIdAsync(request.PhotoId, cancellationToken);
        if (photo is null)
        {
            return UnitResult.Failure<List<Error>>(
                WorkoutErrors.WorkoutPhotoNotFound(request.WorkoutId, request.PhotoId));
        }

        if (!string.Equals(photo.WorkoutId, request.WorkoutId.ToString(), StringComparison.Ordinal))
        {
            return UnitResult.Failure<List<Error>>(
                WorkoutErrors.WorkoutPhotoNotFound(request.WorkoutId, request.PhotoId));
        }

        var isDeleted = await photosRepository.DeleteAsync(request.PhotoId, cancellationToken);
        if (!isDeleted)
        {
            return UnitResult.Failure<List<Error>>(
                WorkoutErrors.WorkoutPhotoNotFound(request.WorkoutId, request.PhotoId));
        }

        var isFileDeleted = await fileStorage.DeleteWorkoutPhotoAsync(photo.Path, cancellationToken);
        if (!isFileDeleted)
        {
            return UnitResult.Failure<List<Error>>(Error.Internal(message: "Failed to delete workout photo file."));
        }

        return UnitResult.Success<List<Error>>();
    }
}

