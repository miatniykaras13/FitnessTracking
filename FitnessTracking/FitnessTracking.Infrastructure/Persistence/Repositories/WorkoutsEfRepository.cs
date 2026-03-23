using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutsEfRepository(FitnessTrackingDbContext dbContext) : IWorkoutsRepository
{
    public async Task<Result<Workout, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(w => w.Id == id.ToString(), cancellationToken);

        return workout is null
            ? Result.Failure<Workout, Error>(WorkoutErrors.NotFound(id))
            : Result.Success<Workout, Error>(workout);
    }

    public async Task<UnitResult<Error>> AddAsync(Workout workout, CancellationToken cancellationToken)
    {
        await dbContext.Workouts.AddAsync(workout, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateAsync(Workout workout, CancellationToken cancellationToken)
    {
        dbContext.Workouts.Update(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.NotFound(id));
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<Result<IReadOnlyList<Workout>, Error>> GetByUserIdAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var workouts = await dbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId.ToString())
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<Workout>, Error>(workouts);
    }

    public async Task<Result<IReadOnlyList<Exercise>, Error>> GetExercisesByWorkoutIdAsync(Guid workoutId,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workoutId.ToString(), cancellationToken);

        return workout is null
            ? Result.Failure<IReadOnlyList<Exercise>, Error>(WorkoutErrors.NotFound(workoutId))
            : Result.Success<IReadOnlyList<Exercise>, Error>(workout.Exercises);
    }

    public async Task<Result<Guid, Error>> AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId.ToString(), cancellationToken);

        var photoId = Guid.NewGuid();
        if (workout is null)
        {
            return Result.Failure<Guid, Error>(WorkoutErrors.NotFound(workoutId));
        }

        // todo: сделать реализацию
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success<Guid, Error>(photoId);
    }
}