using FitnessTracking.Domain.Models;

namespace FitnessTracking.Application.Abstractions;

public interface IWorkoutsRepository
{
    Task<Workout> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Workout workout, CancellationToken cancellationToken);

    Task UpdateAsync(Workout workout, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<Workout>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Exercise>> GetExercisesByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken);
    
    Task AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken cancellationToken);
}