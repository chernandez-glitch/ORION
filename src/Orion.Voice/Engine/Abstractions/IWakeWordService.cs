using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>
/// Detección de la palabra de activación ("Orion"). Soporta varios motores
/// (Manual/OpenWakeWord/Porcupine/Custom) seleccionables por configuración; en
/// esta fase solo el manual está disponible (sin modelos propios).
/// </summary>
public interface IWakeWordService
{
    event EventHandler<string>? WakeWordDetected;

    WakeWordEngine Engine { get; }

    bool IsAvailable { get; }

    Task<Result> StartAsync(string wakeWord, double sensitivity, CancellationToken cancellationToken = default);

    Task<Result> StopAsync(CancellationToken cancellationToken = default);

    /// <summary>Dispara la detección manualmente (motor Manual / demo).</summary>
    void Trigger();
}
