using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Shared.Errors;
using Microsoft.Extensions.Configuration;

namespace FitnessTracking.Infrastructure.Helpers;

public class LocalFileStorage(IConfiguration configuration) : ILocalFileStorage
{
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];

    public async Task<Result<string, Error>> SaveWorkoutPhotoAsync(
        Guid workoutId,
        string fileName,
        byte[] fileContent,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return Result.Failure<string, Error>(
                Error.Validation("photo.extension", "Only jpg, jpeg, png, webp and gif are allowed."));
        }

        var rootPath = GetRootPath();

        var workoutFolder = Path.Combine(rootPath, "workouts", workoutId.ToString());
        Directory.CreateDirectory(workoutFolder);

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(workoutFolder, safeFileName);

        await File.WriteAllBytesAsync(absolutePath, fileContent, cancellationToken);

        var relativePath = $"/uploads/workouts/{workoutId}/{safeFileName}";
        return Result.Success<string, Error>(relativePath);
    }

    public Task<UnitResult<Error>> DeleteWorkoutPhotoAsync(string relativePath, CancellationToken cancellationToken)
    {
        var normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var rootPath = GetRootPath();

       
        if (normalizedPath.StartsWith($"uploads{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = normalizedPath[("uploads".Length + 1)..];
        }

        var absolutePath = Path.Combine(rootPath, normalizedPath);
        if (!File.Exists(absolutePath))
        {
            return Task.FromResult(UnitResult.Success<Error>());
        }

        File.Delete(absolutePath);
        return Task.FromResult(UnitResult.Success<Error>());
    }

    private string GetRootPath()
    {
        var rootPathSetting = configuration["FileStorage:RootPath"] ?? "uploads";
        return Path.IsPathRooted(rootPathSetting)
            ? rootPathSetting
            : Path.Combine(Directory.GetCurrentDirectory(), rootPathSetting);
    }
}



