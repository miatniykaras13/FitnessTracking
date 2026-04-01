using CSharpFunctionalExtensions;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutsRepository : IRepository<Workout, Guid>
{
    Task<Result<IReadOnlyList<Workout>, Error>> GetByUserIdAsync(Guid userId, WorkoutFilter filter,
        SortParameters sortParameters,
        PageParameters pageParameters,
        CancellationToken cancellationToken);
    
    Task<Result<Workout, Error>> GetByIdWithPhotosAsync(
        Guid workoutId,
        CancellationToken cancellationToken);

    Task<Result<int, Error>> GetCountByUserIdWithFilterAsync(
        Guid userId,
        WorkoutFilter filter,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Exercise>, Error>> GetExercisesByWorkoutIdAsync(
        Guid workoutId,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddExerciseAsync(Guid workoutId, Exercise exercise, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateExerciseAsync(
        Guid workoutId,
        string exerciseName,
        Exercise exercise,
        CancellationToken cancellationToken);
    
    Task<UnitResult<Error>> UpdateExercisesAsync(
        Guid workoutId,
        IReadOnlyList<Exercise> exercises,
        CancellationToken cancellationToken);

    Task<bool> DeleteExerciseAsync(
        Guid workoutId,
        string exerciseName,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddSetAsync(
        Guid workoutId,
        string exerciseName,
        Set set,
        CancellationToken cancellationToken);
    
    Task<UnitResult<Error>> UpdateSetsAsync(
        Guid workoutId,
        string exerciseName,
        IReadOnlyList<Set> sets,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        Set set,
        CancellationToken cancellationToken);

    Task<bool> DeleteSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        CancellationToken cancellationToken);
}