using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Internal;
using Orion.Windows.Models;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsCmdService(ILogger<WindowsCmdService> logger) : ICmdService
{
    public async Task<Result<ShellResult>> RunAsync(string command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return Result.Failure<ShellResult>(Error.Validation("Cmd.Empty", "El comando no puede estar vacío."));
        }

        try
        {
            var result = await ProcessRunner.RunAsync("cmd.exe", $"/c {command}", cancellationToken).ConfigureAwait(false);
            logger.LogInformation("CMD ejecutado (exit {ExitCode}).", result.ExitCode);
            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<ShellResult>(Error.Failure("Cmd.Cancelled", "Ejecución cancelada."));
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            logger.LogError(ex, "Fallo al ejecutar CMD.");
            return Result.Failure<ShellResult>(Error.Failure("Cmd.Failed", ex.Message));
        }
    }

    public Result RunAsAdmin(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return Result.Failure(Error.Validation("Cmd.Empty", "El comando no puede estar vacío."));
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k {command}",
                UseShellExecute = true,
                Verb = "runas"
            });

            logger.LogInformation("CMD elevado lanzado.");
            return Result.Success();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            // El usuario canceló el UAC o no hay permisos.
            return Result.Failure(Error.Forbidden("Cmd.Elevation", $"No se pudo ejecutar como administrador: {ex.Message}"));
        }
    }
}
