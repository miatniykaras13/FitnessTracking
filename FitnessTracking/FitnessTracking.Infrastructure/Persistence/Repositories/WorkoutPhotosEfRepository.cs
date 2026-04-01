using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutPhotosEfRepository(FitnessTrackingDbContext dbContext) : IWorkoutPhotosRepository
{
    public async Task<Result<WorkoutPhoto, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workoutPhoto = await dbContext.WorkoutPhotos
            .FindAsync([id.ToString()], cancellationToken);
        if (workoutPhoto is null)
        {
            return Result.Failure<WorkoutPhoto, Error>(WorkoutErrors.WorkoutPhotoNotFound(Guid.Empty, id));
        }

        return Result.Success<WorkoutPhoto, Error>(workoutPhoto);
    }

    public async Task<Result<WorkoutPhoto, Error>> GetByWorkoutIdAndPhotoIdAsync(
        Guid workoutId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var workoutPhoto = await dbContext.WorkoutPhotos
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Id == photoId.ToString() && p.WorkoutId == workoutId.ToString(),
                cancellationToken);

        if (workoutPhoto is null)
        {
            return Result.Failure<WorkoutPhoto, Error>(WorkoutErrors.WorkoutPhotoNotFound(workoutId, photoId));
        }

        return Result.Success<WorkoutPhoto, Error>(workoutPhoto);
    }

    public async Task<UnitResult<Error>> AddAsync(
        WorkoutPhoto photo,
        CancellationToken cancellationToken)
    {
        await dbContext.WorkoutPhotos.AddAsync(photo, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateAsync(WorkoutPhoto photo, CancellationToken cancellationToken)
    {
        dbContext.WorkoutPhotos.Update(photo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var photo = await dbContext.WorkoutPhotos
            .FindAsync([id.ToString()], cancellationToken);

        if (photo is null)
        {
            return false;
        }

        dbContext.WorkoutPhotos.Remove(photo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteByWorkoutIdAndPhotoIdAsync(
        Guid workoutId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var photo = await dbContext.WorkoutPhotos
            .FirstOrDefaultAsync(
                p => p.Id == photoId.ToString() && p.WorkoutId == workoutId.ToString(),
                cancellationToken);

        if (photo is null)
        {
            return false;
        }

        dbContext.WorkoutPhotos.Remove(photo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}