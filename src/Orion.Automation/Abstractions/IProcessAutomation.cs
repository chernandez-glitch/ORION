using Orion.Automation.Models;
using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para arrancar, cerrar y consultar procesos de Windows.</summary>
public interface IProcessAutomation
{
    Task<Result<ProcessInfo>> LaunchAsync(string path, string? arguments = null, CancellationToken cancellationToken = default);

    Task<Result> CloseAsync(string processName, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ProcessInfo>>> FindAsync(string processName, CancellationToken cancellationToken = default);
}
