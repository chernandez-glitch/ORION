using Orion.Shared.Results;

namespace Orion.Voice.Abstractions;

/// <summary>
/// Detección de palabra de activación (p. ej. "Orion"). Implementación en Fase 3.
/// </summary>
public interface IWakeWordDetector
{
    /// <summary>Se dispara cuando se detecta la palabra de activación.</summary>
    event EventHandler<string>? WakeWordDetected;

    Task<Result> StartAsync(string wakeWord, CancellationToken cancellationToken = default);

    Task<Result> StopAsync(CancellationToken cancellationToken = default);
}
