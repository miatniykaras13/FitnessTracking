namespace FitnessTracking.Application.Abstractions.Helpers;

public interface ILocalFileStorage
{
    Task<string?> SaveWorkoutPhotoAsync(
        Guid workoutId,
        string fileName,
        byte[] fileContent,
        CancellationToken cancellationToken);

    Task<bool> DeleteWorkoutPhotoAsync(string relativePath,
        CancellationToken cancellationToken);
}

