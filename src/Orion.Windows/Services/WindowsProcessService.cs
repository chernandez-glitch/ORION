using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Safety;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsProcessService(ISafetyGuard safety, ILogger<WindowsProcessService> logger) : IProcessService
{
    public Task<Result<IReadOnlyList<ProcessDetails>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ProcessDetails> processes = Process.GetProcesses()
            .Select(TryMap)
            .OfType<ProcessDetails>()
            .OrderByDescending(p => p.WorkingSetBytes)
            .ToArray();

        return Task.FromResult(Result.Success(processes));
    }

    public Task<Result<IReadOnlyList<ProcessDetails>>> FindAsync(string name, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ProcessDetails> processes = Process.GetProcessesByName(Normalize(name))
            .Select(TryMap)
            .OfType<ProcessDetails>()
            .ToArray();

        return Task.FromResult(Result.Success(processes));
    }

    public Result<ProcessDetails> Start(string path, string? arguments = null)
    {
        try
        {
            var startInfo = new ProcessStartInfo { FileName = path, UseShellExecute = true };
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                startInfo.Arguments = arguments;
            }

            var process = Process.Start(startInfo);
            logger.LogInformation("Proceso iniciado: {Path}", path);
            return Result.Success(new ProcessDetails(process?.Id ?? 0, path, null, 0, true));
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException or FileNotFoundException)
        {
            return Result.Failure<ProcessDetails>(Error.Failure("Process.StartFailed", $"No se pudo iniciar '{path}': {ex.Message}"));
        }
    }

    public Task<Result> CloseAsync(string name, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(Normalize(name));
        if (processes.Length == 0)
        {
            return Task.FromResult(Result.Failure(Error.NotFound("Process.NotFound", $"No hay procesos '{name}'.")));
        }

        foreach (var process in processes)
        {
            process.CloseMainWindow();
        }

        logger.LogInformation("Cierre ordenado solicitado a {Count} proceso(s) '{Name}'.", processes.Length, name);
        return Task.FromResult(Result.Success());
    }

    public Task<Result> KillAsync(string name, CancellationToken cancellationToken = default)
    {
        var safe = safety.EnsureSafeToKill(name);
        if (safe.IsFailure)
        {
            return Task.FromResult(safe);
        }

        var processes = Process.GetProcessesByName(Normalize(name));
        if (processes.Length == 0)
        {
            return Task.FromResult(Result.Failure(Error.NotFound("Process.NotFound", $"No hay procesos '{name}'.")));
        }

        foreach (var process in processes)
        {
            process.Kill(entireProcessTree: true);
        }

        logger.LogWarning("Proceso(s) '{Name}' terminado(s) forzosamente.", name);
        return Task.FromResult(Result.Success());
    }

    public async Task<Result> RestartAsync(string name, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(Normalize(name));
        if (processes.Length == 0)
        {
            return Result.Failure(Error.NotFound("Process.NotFound", $"No hay procesos '{name}'."));
        }

        string? path = null;
        try
        {
            path = processes[0].MainModule?.FileName;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return Result.Failure(Error.Forbidden("Process.NoAccess", "No se pudo obtener la ruta del proceso para reiniciarlo."));
        }

        var kill = await KillAsync(name, cancellationToken).ConfigureAwait(false);
        if (kill.IsFailure)
        {
            return kill;
        }

        if (path is null)
        {
            return Result.Success();
        }

        var restart = Start(path);
        return restart.IsSuccess ? Result.Success() : Result.Failure(restart.Error);
    }

    public bool Exists(string name) => Process.GetProcessesByName(Normalize(name)).Length > 0;

    public async Task<Result> WaitForExitAsync(string name, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(Normalize(name));
        if (processes.Length == 0)
        {
            return Result.Success();
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            await processes[0].WaitForExitAsync(cts.Token).ConfigureAwait(false);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Error.Failure("Process.Timeout", $"El proceso '{name}' no terminó en el tiempo indicado."));
        }
    }

    public async Task<Result<double>> GetCpuPercentAsync(string name, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(Normalize(name));
        if (processes.Length == 0)
        {
            return Result.Failure<double>(Error.NotFound("Process.NotFound", $"No hay procesos '{name}'."));
        }

        var process = processes[0];
        var startCpu = process.TotalProcessorTime;
        var startTime = Stopwatch.GetTimestamp();

        await Task.Delay(250, cancellationToken).ConfigureAwait(false);

        process.Refresh();
        var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
        var cpuMs = (process.TotalProcessorTime - startCpu).TotalMilliseconds;
        var percent = elapsedMs > 0 ? Math.Clamp(cpuMs / (elapsedMs * Environment.ProcessorCount) * 100, 0, 100) : 0;

        return Result.Success(Math.Round(percent, 1));
    }

    public Result<double> GetRamMb(string name)
    {
        var processes = Process.GetProcessesByName(Normalize(name));
        return processes.Length == 0
            ? Result.Failure<double>(Error.NotFound("Process.NotFound", $"No hay procesos '{name}'."))
            : Result.Success(Math.Round(processes[0].WorkingSet64 / (1024d * 1024d), 1));
    }

    private static string Normalize(string name) =>
        name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? name[..^4] : name;

    private static ProcessDetails? TryMap(Process process)
    {
        try
        {
            return new ProcessDetails(process.Id, process.ProcessName, SafeTitle(process), process.WorkingSet64, SafeResponding(process));
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }

    private static string? SafeTitle(Process process)
    {
        try
        {
            return string.IsNullOrEmpty(process.MainWindowTitle) ? null : process.MainWindowTitle;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static bool SafeResponding(Process process)
    {
        try
        {
            return process.Responding;
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return true;
        }
    }
}
