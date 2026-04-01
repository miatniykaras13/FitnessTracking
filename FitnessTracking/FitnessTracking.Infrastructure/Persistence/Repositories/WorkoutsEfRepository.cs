using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Constants;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Models;
using FitnessTracking.Infrastructure.Extensions;
using FitnessTracking.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutsEfRepository(FitnessTrackingDbContext dbContext) : IWorkoutsRepository
{
    public async Task<Workout?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);

        return workout;
    }

    public async Task<UnitResult<Error>> AddAsync(Workout workout, CancellationToken cancellationToken)
    {
        await dbContext.Workouts.AddAsync(workout, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<bool> UpdateAsync(Workout workout, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Workouts.AnyAsync(w => w.Id == workout.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        dbContext.Workouts.Update(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);

        if (workout is null)
        {
            return false;
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Workout>> GetByUserIdAsync(Guid userId,
        WorkoutFilter filter,
        SortParameters sortParameters,
        PageParameters pageParameters,
        CancellationToken cancellationToken)
    {
        var workouts = await dbContext.Workouts
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
        var workout = await dbContext.Workouts
            .AsNoTracking()
            .Include(w => w.ProgressPhotos)
            .FirstOrDefaultAsync(w => w.Id.Equals(workoutId.ToString()), cancellationToken);

        return workout;
    }

    public async Task<int> GetCountByUserIdWithFilterAsync(Guid userId,
        WorkoutFilter filter,
        CancellationToken cancellationToken)
    {
        var count = await dbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId.Equals(userId.ToString()))
            .Filter(filter)
            .CountAsync(cancellationToken);

        return count;
    }


    public async Task<IReadOnlyList<Exercise>?> GetExercisesByWorkoutIdAsync(
        Guid workoutId,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return null;
        }

        return workout.Exercises;
    }

    public async Task<UnitResult<Error>> AddExerciseAsync(
        Guid workoutId,
        Exercise exercise,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        var hasDuplicateExercise = workout.Exercises.Any(e =>
            string.Equals(e.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        if (hasDuplicateExercise)
        {
            return UnitResult.Failure(WorkoutErrors.ExerciseAlreadyExists(workoutId, exercise.Name));
        }

        workout.Exercises.Add(exercise);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<bool> UpdateExerciseAsync(
        Guid workoutId,
        string exerciseName,
        Exercise exercise,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
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

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateExercisesAsync(
        Guid workoutId,
        IReadOnlyList<Exercise> exercises,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return false;
        }

        workout.Exercises = exercises.ToList();

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }


    public async Task<bool> DeleteExerciseAsync(
        Guid workoutId,
        string exerciseName,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
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
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<UnitResult<Error>> AddSetAsync(
        Guid workoutId,
        string exerciseName,
        Set set,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return UnitResult.Failure(WorkoutErrors.ExerciseNotFound(workoutId, exerciseName));
        }

        exercise.Sets.Add(set);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<bool> UpdateSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        Set set,
        CancellationToken cancellationToken)
    {
        if (setIndex < ValidationConstants.MinZeroBasedIndex)
            return false;

        var workout = await dbContext.Workouts
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
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSetsAsync(
        Guid workoutId,
        string exerciseName,
        IReadOnlyList<Set> sets,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
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
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        CancellationToken cancellationToken)
    {
        if (setIndex < ValidationConstants.MinZeroBasedIndex)
        {
            return false;
        }

        var workout = await dbContext.Workouts
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
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}