using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Constants;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Models;
using FitnessTracking.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutsEfRepository(FitnessTrackingDbContext dbContext)
    : Repository<Workout, Guid>(dbContext, dbContext.Workouts), IWorkoutsRepository
{
    public async Task<IReadOnlyList<Workout>> GetByUserIdAsync(Guid userId, WorkoutFilter filter, SortParameters sortParameters, PageParameters pageParameters, CancellationToken cancellationToken)
    {
        var workouts = await DbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId.Equals(userId.ToString()))
            .Filter(filter)
            .Sort(sortParameters)
            .Page(pageParameters)
            .ToListAsync(cancellationToken);

        return workouts;
    }

    public async Task<Workout?> GetByIdWithPhotosAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .AsNoTracking()
            .Include(w => w.ProgressPhotos)
            .FirstOrDefaultAsync(w => w.Id.Equals(workoutId.ToString()), cancellationToken);

        return workout;
    }

    public async Task<int> GetCountByUserIdWithFilterAsync(Guid userId, WorkoutFilter filter, CancellationToken cancellationToken)
    {
        var count = await DbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId.Equals(userId.ToString()))
            .Filter(filter)
            .CountAsync(cancellationToken);

        return count;
    }

    public async Task<IReadOnlyList<Exercise>?> GetExercisesByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return null;
        }

        return workout.Exercises;
    }

    public async Task<Exercise?> AddExerciseAsync(Guid workoutId, Exercise exercise, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return null;
        }

        var hasDuplicateExercise = workout.Exercises.Any(e =>
            string.Equals(e.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        if (hasDuplicateExercise)
        {
            return null;
        }

        workout.Exercises.Add(exercise);
        await DbContext.SaveChangesAsync(cancellationToken);
        return exercise;
    }

    public async Task<bool> UpdateExerciseAsync(Guid workoutId, string exerciseName, Exercise exercise, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return false;
        }

        var existingExercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (existingExercise is null)
        {
            return false;
        }

        existingExercise.Name = exercise.Name;
        existingExercise.Sets = exercise.Sets;

        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateExercisesAsync(Guid workoutId, IReadOnlyList<Exercise> exercises, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return false;
        }

        workout.Exercises = exercises.ToList();

        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }


    public async Task<bool> DeleteExerciseAsync(Guid workoutId, string exerciseName, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return false;
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return false;
        }

        workout.Exercises.Remove(exercise);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Set?> AddSetAsync(Guid workoutId, string exerciseName, Set set, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return null;
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return null;
        }

        exercise.Sets.Add(set);
        await DbContext.SaveChangesAsync(cancellationToken);
        return set;
    }

    public async Task<bool> UpdateSetAsync(Guid workoutId, string exerciseName, int setIndex, Set set, CancellationToken cancellationToken)
    {
        if (setIndex < ValidationConstants.MinZeroBasedIndex)
            return false;

        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return false;
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return false;
        }

        if (setIndex >= exercise.Sets.Count)
        {
            return false;
        }

        exercise.Sets[setIndex] = set;
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSetsAsync(Guid workoutId, string exerciseName, IReadOnlyList<Set> sets, CancellationToken cancellationToken)
    {
        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return false;
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return false;
        }

        exercise.Sets = sets.ToList();
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteSetAsync(Guid workoutId, string exerciseName, int setIndex, CancellationToken cancellationToken)
    {
        if (setIndex < ValidationConstants.MinZeroBasedIndex)
        {
            return false;
        }

        var workout = await DbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return false;
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return false;
        }

        if (setIndex >= exercise.Sets.Count)
        {
            return false;
        }

        exercise.Sets.RemoveAt(setIndex);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}