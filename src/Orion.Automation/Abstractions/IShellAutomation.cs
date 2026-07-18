using Orion.Automation.Models;
using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para ejecutar comandos en PowerShell o CMD.</summary>
public interface IShellAutomation
{
    Task<Result<ShellResult>> RunAsync(ShellKind shell, string command, CancellationToken cancellationToken = default);
}
