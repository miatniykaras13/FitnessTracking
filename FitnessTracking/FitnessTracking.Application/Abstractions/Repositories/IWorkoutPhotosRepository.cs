using FitnessTracking.Domain.Models;
using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutPhotosRepository : IRepository<WorkoutPhoto, Guid>
{
    Task<Result<WorkoutPhoto, Error>> GetByWorkoutIdAndPhotoIdAsync(
        Guid workoutId,
        Guid photoId,
        CancellationToken cancellationToken);

    Task<bool> DeleteByWorkoutIdAndPhotoIdAsync(
        Guid workoutId,
        Guid photoId,
        CancellationToken cancellationToken);
}