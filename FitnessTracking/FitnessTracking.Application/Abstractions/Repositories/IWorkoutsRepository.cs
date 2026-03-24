using CSharpFunctionalExtensions;
using FitnessTracking.Application.Filters;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutsRepository
{
    Task<Result<Workout, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddAsync(Workout workout, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateAsync(Workout workout, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    
    Task<Result<IReadOnlyList<Workout>, Error>> GetByUserIdAsync(Guid userId, WorkoutFilter filter,
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

    Task<UnitResult<Error>> DeleteExerciseAsync(
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

    Task<UnitResult<Error>> DeleteSetAsync(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken cancellationToken);
}