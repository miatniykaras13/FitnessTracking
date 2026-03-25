using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Helpers;

public interface ILocalFileStorage
{
    Task<Result<string, Error>> SaveWorkoutPhotoAsync(
        Guid workoutId,
        string fileName,
        byte[] fileContent,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteWorkoutPhotoAsync(
        string relativePath,
        CancellationToken cancellationToken);
}

