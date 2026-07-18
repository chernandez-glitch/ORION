using Orion.Automation.Models;
using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para enumerar y enfocar ventanas.</summary>
public interface IWindowAutomation
{
    Task<Result<IReadOnlyList<ProcessInfo>>> ListWindowsAsync(CancellationToken cancellationToken = default);

    Task<Result> FocusAsync(string windowTitle, CancellationToken cancellationToken = default);
}
