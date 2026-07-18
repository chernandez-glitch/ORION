using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para simular entrada de teclado y mouse.</summary>
public interface IInputAutomation
{
    Task<Result> SendKeysAsync(string keys, CancellationToken cancellationToken = default);

    Task<Result> MoveMouseAsync(int x, int y, CancellationToken cancellationToken = default);
}
