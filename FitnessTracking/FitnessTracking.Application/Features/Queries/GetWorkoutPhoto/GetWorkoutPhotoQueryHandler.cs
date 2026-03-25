using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;

public class GetWorkoutPhotoQueryHandler(
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

        var photoResult = await photosRepository.GetByWorkoutIdAndPhotoIdAsync(
            request.WorkoutId,
            request.PhotoId,
            cancellationToken);

        if (photoResult.IsFailure)
        {
            return Result.Failure<GetWorkoutPhotoResponse, List<Error>>(photoResult.Error);
        }

        var photo = photoResult.Value;
        return Result.Success<GetWorkoutPhotoResponse, List<Error>>(
            new GetWorkoutPhotoResponse(
                Guid.Parse(photo.Id),
                Guid.Parse(photo.WorkoutId),
                photo.Path,
                photo.CreatedAt));
    }
}

