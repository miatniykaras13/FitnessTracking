using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Models;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutsRepository : IRepository<Workout, Guid>
{
    Task<IReadOnlyList<Workout>> GetByUserIdAsync(Guid userId, WorkoutFilter filter, SortParameters sortParameters, PageParameters pageParameters, CancellationToken cancellationToken);

    Task<Workout?> GetByIdWithPhotosAsync(Guid workoutId, CancellationToken cancellationToken);

    Task<int> GetCountByUserIdWithFilterAsync(Guid userId, WorkoutFilter filter, CancellationToken cancellationToken);

    Task<IReadOnlyList<Exercise>?> GetExercisesByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken);

    Task<Exercise?> AddExerciseAsync(Guid workoutId, Exercise exercise, CancellationToken cancellationToken);

    Task<bool> UpdateExerciseAsync(Guid workoutId, string exerciseName, Exercise exercise, CancellationToken cancellationToken);

    Task<bool> UpdateExercisesAsync(Guid workoutId, IReadOnlyList<Exercise> exercises, CancellationToken cancellationToken);

    Task<bool> DeleteExerciseAsync(Guid workoutId, string exerciseName, CancellationToken cancellationToken);

    Task<Set?> AddSetAsync(Guid workoutId, string exerciseName, Set set, CancellationToken cancellationToken);

    Task<bool> UpdateSetsAsync(Guid workoutId, string exerciseName, IReadOnlyList<Set> sets, CancellationToken cancellationToken);

    Task<bool> UpdateSetAsync(Guid workoutId, string exerciseName, int setIndex, Set set, CancellationToken cancellationToken);

    Task<bool> DeleteSetAsync(Guid workoutId, string exerciseName, int setIndex, CancellationToken cancellationToken);
}