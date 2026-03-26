using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
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
    public async Task<Result<Workout, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);
        if (workout is null)
        {
            return Result.Failure<Workout, Error>(WorkoutErrors.WorkoutNotFound(id));
        }

        return Result.Success<Workout, Error>(workout);
    }

    public async Task<UnitResult<Error>> AddAsync(Workout workout, CancellationToken cancellationToken)
    {
        await dbContext.Workouts.AddAsync(workout, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateAsync(Workout photo, CancellationToken cancellationToken)
    {
        dbContext.Workouts.Update(photo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([id.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.WorkoutNotFound(id));
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<Result<IReadOnlyList<Workout>, Error>> GetByUserIdAsync(
        Guid userId,
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

        return Result.Success<IReadOnlyList<Workout>, Error>(workouts);
    }

    public async Task<Result<Workout, Error>> GetByIdWithPhotosAsync(Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .AsNoTracking()
            .Include(w => w.ProgressPhotos)
            .FirstOrDefaultAsync(w => w.Id.Equals(workoutId.ToString()), cancellationToken);

        if (workout is null)
        {
            return Result.Failure<Workout, Error>(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        return Result.Success<Workout, Error>(workout);
    }

    public async Task<Result<int, Error>> GetCountByUserIdWithFilterAsync(
        Guid userId,
        WorkoutFilter filter,
        CancellationToken cancellationToken) =>
        await dbContext.Workouts
            .AsNoTracking()
            .Where(w => w.UserId.Equals(userId.ToString()))
            .Filter(filter)
            .CountAsync(cancellationToken);


    public async Task<Result<IReadOnlyList<Exercise>, Error>> GetExercisesByWorkoutIdAsync(
        Guid workoutId,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);
        if (workout is null)
        {
            return Result.Failure<IReadOnlyList<Exercise>, Error>(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        return Result.Success<IReadOnlyList<Exercise>, Error>(workout.Exercises);
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

    public async Task<UnitResult<Error>> UpdateExerciseAsync(
        Guid workoutId,
        string exerciseName,
        Exercise exercise,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        var existingExercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, exerciseName, StringComparison.OrdinalIgnoreCase));
        if (existingExercise is null)
        {
            return UnitResult.Failure(WorkoutErrors.ExerciseNotFound(workoutId, exerciseName));
        }

        var hasNameConflict = workout.Exercises.Any(e =>
            !ReferenceEquals(e, existingExercise) &&
            string.Equals(e.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        if (hasNameConflict)
        {
            return UnitResult.Failure(WorkoutErrors.ExerciseAlreadyExists(workoutId, exercise.Name));
        }

        existingExercise.Name = exercise.Name;
        existingExercise.Sets = exercise.Sets;

        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateExercisesAsync(
        Guid workoutId,
        IReadOnlyList<Exercise> exercises,
        CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FindAsync([workoutId.ToString()], cancellationToken);

        if (workout is null)
        {
            return UnitResult.Failure(WorkoutErrors.WorkoutNotFound(workoutId));
        }

        var duplicateName = exercises
            .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1)?.Key;
        if (duplicateName is not null)
        {
            return UnitResult.Failure(WorkoutErrors.ExerciseAlreadyExists(workoutId, duplicateName));
        }

        workout.Exercises = exercises.ToList();

        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }


    public async Task<UnitResult<Error>> DeleteExerciseAsync(
        Guid workoutId,
        string exerciseName,
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

        workout.Exercises.Remove(exercise);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
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

    public async Task<UnitResult<Error>> UpdateSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        Set set,
        CancellationToken cancellationToken)
    {
        if (setIndex < 0)
        {
            return UnitResult.Failure(WorkoutErrors.InvalidSetIndex(setIndex));
        }

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

        if (setIndex >= exercise.Sets.Count)
        {
            return UnitResult.Failure(WorkoutErrors.SetNotFound(workoutId, exerciseName, setIndex));
        }

        exercise.Sets[setIndex] = set;
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateSetsAsync(
        Guid workoutId,
        string exerciseName,
        IReadOnlyList<Set> sets,
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

        exercise.Sets = sets.ToList();
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        CancellationToken cancellationToken)
    {
        if (setIndex < 0)
        {
            return UnitResult.Failure(WorkoutErrors.InvalidSetIndex(setIndex));
        }

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

        if (setIndex >= exercise.Sets.Count)
        {
            return UnitResult.Failure(WorkoutErrors.SetNotFound(workoutId, exerciseName, setIndex));
        }

        exercise.Sets.RemoveAt(setIndex);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }
}