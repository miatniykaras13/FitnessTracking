using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;

public class GetWorkoutPhotoQueryHandler(
    IWorkoutsRepository workoutsRepository,
    IWorkoutPhotosRepository photosRepository,
    IValidator<GetWorkoutPhotoQuery> validator)
    : IQueryHandler<GetWorkoutPhotoQuery, Result<GetWorkoutPhotoResponse, List<Error>>>
{
    public async Task<Result<GetWorkoutPhotoResponse, List<Error>>> Handle(
        GetWorkoutPhotoQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(WorkoutPhoto).ToLower());
        }

        var workout = await workoutsRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
        if (workout is null)
        {
            return Result.Failure<GetWorkoutPhotoResponse, List<Error>>(
                WorkoutErrors.WorkoutNotFound(request.WorkoutId));
        }


        var photo = await photosRepository.GetByIdAsync(request.PhotoId, cancellationToken);
        if (photo is null)
        {
            return Result.Failure<GetWorkoutPhotoResponse, List<Error>>(
                WorkoutErrors.WorkoutPhotoNotFound(request.WorkoutId, request.PhotoId));
        }

        if (!string.Equals(photo.WorkoutId, request.WorkoutId.ToString(), StringComparison.Ordinal))
        {
            return Result.Failure<GetWorkoutPhotoResponse, List<Error>>(
                WorkoutErrors.WorkoutPhotoNotFound(request.WorkoutId, request.PhotoId));
        }

        return Result.Success<GetWorkoutPhotoResponse, List<Error>>(
            new GetWorkoutPhotoResponse(
                Guid.Parse(photo.Id),
                Guid.Parse(photo.WorkoutId),
                photo.Path,
                photo.CreatedAt));
    }
}

