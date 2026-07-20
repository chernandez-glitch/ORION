using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Ejecuta PowerShell y captura su salida/errores. Cancelable vía token.</summary>
public interface IPowerShellService
{
    Task<Result<ShellResult>> RunScriptAsync(string script, CancellationToken cancellationToken = default);

    Task<Result<ShellResult>> RunFileAsync(string scriptPath, string? arguments = null, CancellationToken cancellationToken = default);
}
