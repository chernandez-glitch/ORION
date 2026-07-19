using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Infrastructure.Automation;

/// <summary>
/// Adaptador real de <see cref="IPowerAutomation"/> para Windows (shutdown.exe y
/// LockWorkStation vía rundll32).
/// </summary>
public sealed class WindowsPowerAutomation(ILogger<WindowsPowerAutomation> logger) : IPowerAutomation
{
    public Task<Result> ShutdownAsync(TimeSpan delay, CancellationToken cancellationToken = default) =>
        RunAsync("shutdown.exe", $"/s /t {Seconds(delay)}", "apagar");

    public Task<Result> RestartAsync(TimeSpan delay, CancellationToken cancellationToken = default) =>
        RunAsync("shutdown.exe", $"/r /t {Seconds(delay)}", "reiniciar");

    public Task<Result> LockAsync(CancellationToken cancellationToken = default) =>
        RunAsync("rundll32.exe", "user32.dll,LockWorkStation", "bloquear");

    private Task<Result> RunAsync(string fileName, string arguments, string action)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            logger.LogError(ex, "No se pudo {Action} el equipo.", action);
            return Task.FromResult(Result.Failure(
                Error.Failure("Power.Failed", $"No se pudo {action} el equipo: {ex.Message}")));
        }
    }

    private static string Seconds(TimeSpan delay) =>
        ((int)Math.Max(0, delay.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
}
