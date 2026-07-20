using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsExplorerService(ILogger<WindowsExplorerService> logger) : IExplorerService
{
    public Result OpenFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            return Result.Failure(Error.NotFound("Explorer.FolderNotFound", $"No existe la carpeta '{path}'."));
        }

        return Launch("explorer.exe", $"\"{path}\"", $"abrir carpeta '{path}'");
    }

    public Result OpenFolders(IEnumerable<string> paths)
    {
        var errors = new List<string>();
        foreach (var path in paths)
        {
            var result = OpenFolder(path);
            if (result.IsFailure)
            {
                errors.Add(result.Error.Message);
            }
        }

        return errors.Count == 0
            ? Result.Success()
            : Result.Failure(Error.Failure("Explorer.PartialFailure", string.Join("; ", errors)));
    }

    public Result SelectFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return Result.Failure(Error.NotFound("Explorer.FileNotFound", $"No existe el archivo '{filePath}'."));
        }

        return Launch("explorer.exe", $"/select,\"{filePath}\"", $"seleccionar '{filePath}'");
    }

    public Result OpenLocation(string path) =>
        Directory.Exists(path) ? OpenFolder(path)
        : File.Exists(path) ? SelectFile(path)
        : Result.Failure(Error.NotFound("Explorer.NotFound", $"No existe '{path}'."));

    public Result ShowProperties(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            return Result.Failure(Error.NotFound("Explorer.NotFound", $"No existe '{path}'."));
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "properties"
            });
            return Result.Success();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            return Result.Failure(Error.Failure("Explorer.PropertiesFailed", ex.Message));
        }
    }

    private Result Launch(string fileName, string arguments, string action)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = true });
            logger.LogInformation("Explorer: {Action}.", action);
            return Result.Success();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            return Result.Failure(Error.Failure("Explorer.Failed", ex.Message));
        }
    }
}
