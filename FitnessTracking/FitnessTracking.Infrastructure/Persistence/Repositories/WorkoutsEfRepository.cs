using FitnessTracking.Application.Abstractions;
using FitnessTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutsEfRepository(FitnessTrackingDbContext dbContext) : IWorkoutsRepository
{
    public async Task<Workout> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(w => w.Id == id.ToString(), cancellationToken);

        if (workout is null)
        {
            throw new KeyNotFoundException($"Workout with id {id} was not found.");
        }

        return workout;
    }

    public async Task AddAsync(Workout workout, CancellationToken cancellationToken)
    {
        await dbContext.Workouts.AddAsync(workout, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Workout workout, CancellationToken cancellationToken)
    {
        dbContext.Workouts.Update(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);

        if (workout is null)
        {
            return;
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Workout>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId == userId.ToString())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Exercise>> GetExercisesByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workoutId.ToString(), cancellationToken);

        if (workout is null)
        {
            throw new KeyNotFoundException($"Workout with id {workoutId} was not found.");
        }

        return workout.Exercises;
    }

    public async Task AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId.ToString(), cancellationToken);

        if (workout is null)
        {
            throw new KeyNotFoundException($"Workout with id {workoutId} was not found.");
        }
        //todo: сделать реализацию 
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}