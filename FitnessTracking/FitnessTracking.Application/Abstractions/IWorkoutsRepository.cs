using CSharpFunctionalExtensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions;

public interface IWorkoutsRepository
{
    Task<Result<Workout, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddAsync(Workout workout, CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateAsync(Workout workout, CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    
    Task<Result<IReadOnlyList<Workout>, Error>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Exercise>, Error>> GetExercisesByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken);
    
    Task<Result<Guid, Error>> AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken cancellationToken);
}