using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Shared.Constants;
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

    public async Task<string?> SaveWorkoutPhotoAsync(
        Guid workoutId,
        string fileName,
        byte[] fileContent,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return null;
        }

        var rootPath = GetRootPath();

        var workoutFolder = Path.Combine(rootPath, FileStorageConstants.WorkoutsFolderName, workoutId.ToString());
        Directory.CreateDirectory(workoutFolder);

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(workoutFolder, safeFileName);

        await File.WriteAllBytesAsync(absolutePath, fileContent, cancellationToken);

        var relativePath =
            $"{FileStorageConstants.UploadsRequestPath}/{FileStorageConstants.WorkoutsFolderName}/{workoutId}/{safeFileName}";
        return relativePath;
    }

    public Task<bool> DeleteWorkoutPhotoAsync(string relativePath, CancellationToken cancellationToken)
    {
        var normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var rootPath = GetRootPath();

       
        if (normalizedPath.StartsWith(
                $"{FileStorageConstants.DefaultRootFolderName}{Path.DirectorySeparatorChar}",
                StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = normalizedPath[(FileStorageConstants.DefaultRootFolderName.Length + 1)..];
        }

        var absolutePath = Path.Combine(rootPath, normalizedPath);
        if (!File.Exists(absolutePath))
        {
            return Task.FromResult(true);
        }

        File.Delete(absolutePath);
        return Task.FromResult(true);
    }

    private string GetRootPath()
    {
        var rootPathSetting =
            configuration[FileStorageConstants.RootPathConfigKey] ?? FileStorageConstants.DefaultRootFolderName;
        return Path.IsPathRooted(rootPathSetting)
            ? rootPathSetting
            : Path.Combine(Directory.GetCurrentDirectory(), rootPathSetting);
    }
}



