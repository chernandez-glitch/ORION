using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsSystemService(ILogger<WindowsSystemService> logger) : ISystemService
{
    public Result Shutdown(TimeSpan delay) => Run("shutdown.exe", $"/s /t {Seconds(delay)}", "apagar");

    public Result Restart(TimeSpan delay) => Run("shutdown.exe", $"/r /t {Seconds(delay)}", "reiniciar");

    public Result Suspend() => Run("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0", "suspender");

    public Result LogOff() => Run("shutdown.exe", "/l", "cerrar sesión");

    public Result Lock()
    {
        NativeMethods.LockWorkStation();
        logger.LogInformation("Sistema: bloquear.");
        return Result.Success();
    }

    public Result<SystemInfo> GetSystemInfo() =>
        Result.Success(new SystemInfo(
            Environment.UserName,
            Environment.MachineName,
            Environment.OSVersion.Version.ToString(),
            RuntimeInformation.OSDescription));

    private Result Run(string fileName, string arguments, string action)
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

            logger.LogWarning("Sistema: {Action}.", action);
            return Result.Success();
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return Result.Failure(Error.Failure("System.Failed", $"No se pudo {action}: {ex.Message}"));
        }
    }

    private static string Seconds(TimeSpan delay) =>
        ((int)Math.Max(0, delay.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
}
