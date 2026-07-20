using System.IO.Compression;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Safety;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsFileSystemService(ISafetyGuard safety, ILogger<WindowsFileSystemService> logger) : IFileSystemService
{
    public Result CreateFolder(string path) => Guarded("crear carpeta", () => Directory.CreateDirectory(path));

    public Result DeleteFolder(string path, bool recursive = true)
    {
        var safe = safety.EnsureSafeToDelete(path);
        if (safe.IsFailure)
        {
            return safe;
        }

        return Guarded("eliminar carpeta", () =>
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        });
    }

    public Result MoveFolder(string source, string destination) =>
        Guarded("mover carpeta", () => Directory.Move(source, destination));

    public Result CopyFolder(string source, string destination) =>
        Guarded("copiar carpeta", () => CopyDirectory(source, destination));

    public Result CreateFile(string path, string? content = null) =>
        Guarded("crear archivo", () => File.WriteAllText(path, content ?? string.Empty));

    public Result DeleteFile(string path)
    {
        var safe = safety.EnsureSafeToDelete(path);
        if (safe.IsFailure)
        {
            return safe;
        }

        return Guarded("eliminar archivo", () =>
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        });
    }

    public Result CopyFile(string source, string destination, bool overwrite = true) =>
        Guarded("copiar archivo", () => File.Copy(source, destination, overwrite));

    public Result MoveFile(string source, string destination) =>
        Guarded("mover archivo", () => File.Move(source, destination, overwrite: true));

    public Result Rename(string path, string newName) => Guarded("renombrar", () =>
    {
        var directory = Path.GetDirectoryName(path) ?? string.Empty;
        var target = Path.Combine(directory, newName);
        if (Directory.Exists(path))
        {
            Directory.Move(path, target);
        }
        else
        {
            File.Move(path, target, overwrite: true);
        }
    });

    public Result Zip(string sourceDirectory, string zipPath) => Guarded("comprimir", () =>
    {
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        ZipFile.CreateFromDirectory(sourceDirectory, zipPath);
    });

    public Result Unzip(string zipPath, string destinationDirectory) =>
        Guarded("descomprimir", () => ZipFile.ExtractToDirectory(zipPath, destinationDirectory, overwriteFiles: true));

    private Result Guarded(string action, Action operation)
    {
        try
        {
            operation();
            logger.LogInformation("FileSystem: {Action}.", action);
            return Result.Success();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            logger.LogError(ex, "FileSystem falló: {Action}.", action);
            return Result.Failure(Error.Failure("FileSystem.Failed", $"No se pudo {action}: {ex.Message}"));
        }
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (var file in Directory.GetFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}
