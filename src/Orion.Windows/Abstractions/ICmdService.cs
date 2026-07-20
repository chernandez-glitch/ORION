using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Ejecuta comandos de CMD.</summary>
public interface ICmdService
{
    Task<Result<ShellResult>> RunAsync(string command, CancellationToken cancellationToken = default);

    /// <summary>Ejecuta un comando elevado (UAC). No captura salida (ShellExecute).</summary>
    Result RunAsAdmin(string command);
}
