using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Internal;
using Orion.Windows.Models;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsPowerShellService(ILogger<WindowsPowerShellService> logger) : IPowerShellService
{
    public async Task<Result<ShellResult>> RunScriptAsync(string script, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            return Result.Failure<ShellResult>(Error.Validation("PowerShell.Empty", "El script no puede estar vacío."));
        }

        var encoded = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
        return await RunAsync($"-NoProfile -NonInteractive -EncodedCommand {encoded}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result<ShellResult>> RunFileAsync(string scriptPath, string? arguments = null, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(scriptPath))
        {
            return Result.Failure<ShellResult>(Error.NotFound("PowerShell.FileNotFound", $"No existe el script '{scriptPath}'."));
        }

        return await RunAsync($"-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"{scriptPath}\" {arguments}", cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<Result<ShellResult>> RunAsync(string arguments, CancellationToken cancellationToken)
    {
        try
        {
            var result = await ProcessRunner.RunAsync("powershell.exe", arguments, cancellationToken).ConfigureAwait(false);
            logger.LogInformation("PowerShell ejecutado (exit {ExitCode}).", result.ExitCode);
            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<ShellResult>(Error.Failure("PowerShell.Cancelled", "Ejecución cancelada."));
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            logger.LogError(ex, "Fallo al ejecutar PowerShell.");
            return Result.Failure<ShellResult>(Error.Failure("PowerShell.Failed", ex.Message));
        }
    }
}
