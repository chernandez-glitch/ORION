using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Automation.Abstractions;
using Orion.Automation.Models;
using Orion.Shared.Results;

namespace Orion.Infrastructure.Automation;

/// <summary>
/// Adaptador real de <see cref="IProcessAutomation"/> para Windows. Usa
/// <see cref="Process"/> con ShellExecute, lo que permite abrir aplicaciones por
/// nombre, carpetas (Explorer) y URLs (navegador predeterminado).
/// </summary>
public sealed class WindowsProcessAutomation(ILogger<WindowsProcessAutomation> logger) : IProcessAutomation
{
    public Task<Result<ProcessInfo>> LaunchAsync(string path, string? arguments = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };

            if (!string.IsNullOrWhiteSpace(arguments))
            {
                startInfo.Arguments = arguments;
            }

            var process = Process.Start(startInfo);
            var info = new ProcessInfo(process?.Id ?? 0, path, string.Empty);
            logger.LogInformation("Proceso lanzado: {Path} {Args}", path, arguments);
            return Task.FromResult(Result.Success(info));
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException or FileNotFoundException)
        {
            logger.LogWarning(ex, "No se pudo lanzar '{Path}'.", path);
            return Task.FromResult(Result.Failure<ProcessInfo>(
                Error.Failure("Process.LaunchFailed", $"No se pudo abrir '{path}': {ex.Message}")));
        }
    }

    public Task<Result> CloseAsync(string processName, CancellationToken cancellationToken = default)
    {
        try
        {
            var processes = Process.GetProcessesByName(NormalizeName(processName));
            if (processes.Length == 0)
            {
                return Task.FromResult(Result.Failure(
                    Error.NotFound("Process.NotFound", $"No hay procesos '{processName}' en ejecución.")));
            }

            foreach (var process in processes)
            {
                process.Kill(entireProcessTree: true);
            }

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return Task.FromResult(Result.Failure(
                Error.Failure("Process.CloseFailed", $"No se pudo cerrar '{processName}': {ex.Message}")));
        }
    }

    public Task<Result<IReadOnlyList<ProcessInfo>>> FindAsync(string processName, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(NormalizeName(processName));
        IReadOnlyList<ProcessInfo> infos = processes
            .Select(p => new ProcessInfo(p.Id, p.ProcessName, SafeTitle(p)))
            .ToArray();

        return Task.FromResult(Result.Success(infos));
    }

    private static string NormalizeName(string processName) =>
        processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            ? processName[..^4]
            : processName;

    private static string SafeTitle(Process process)
    {
        try
        {
            return process.MainWindowTitle;
        }
        catch (InvalidOperationException)
        {
            return string.Empty;
        }
    }
}
