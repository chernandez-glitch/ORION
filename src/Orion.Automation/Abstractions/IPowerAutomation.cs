using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para acciones de energía del equipo (apagar, reiniciar, bloquear).</summary>
public interface IPowerAutomation
{
    Task<Result> ShutdownAsync(TimeSpan delay, CancellationToken cancellationToken = default);

    Task<Result> RestartAsync(TimeSpan delay, CancellationToken cancellationToken = default);

    Task<Result> LockAsync(CancellationToken cancellationToken = default);
}
