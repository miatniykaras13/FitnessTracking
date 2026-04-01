using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutPhotosEfRepository(FitnessTrackingDbContext dbContext) : IWorkoutPhotosRepository
{
    public async Task<WorkoutPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workoutPhoto = await dbContext.WorkoutPhotos
            .FindAsync([id.ToString()], cancellationToken);

        return workoutPhoto;
    }


    public async Task<UnitResult<Error>> AddAsync(
        WorkoutPhoto photo,
        CancellationToken cancellationToken)
    {
        await dbContext.WorkoutPhotos.AddAsync(photo, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<bool> UpdateAsync(WorkoutPhoto workout, CancellationToken cancellationToken)
    {
        var exists = await dbContext.WorkoutPhotos.AnyAsync(p => p.Id == workout.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        dbContext.WorkoutPhotos.Update(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
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

}