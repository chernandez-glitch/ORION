using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// Detección de wake word seleccionable por configuración. Solo el motor Manual
/// está disponible en esta fase (OpenWakeWord/Porcupine/Custom requieren modelos,
/// no implementados todavía).
/// </summary>
internal sealed class WakeWordService(IVoiceConfigurationService configuration, ILogger<WakeWordService> logger) : IWakeWordService
{
    public event EventHandler<string>? WakeWordDetected;

    public WakeWordEngine Engine => configuration.Options.WakeWordEngine;

    public bool IsAvailable => Engine == WakeWordEngine.Manual;

    public Task<Result> StartAsync(string wakeWord, double sensitivity, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return Task.FromResult(Result.Failure(Error.Failure("Voice.WakeWordUnavailable",
                $"El motor de wake word '{Engine}' aún no está disponible (requiere modelos).")));
        }

        logger.LogInformation("Wake word '{WakeWord}' en escucha (motor {Engine}, sensibilidad {Sensitivity}).", wakeWord, Engine, sensitivity);
        return Task.FromResult(Result.Success());
    }

    public Task<Result> StopAsync(CancellationToken cancellationToken = default) => Task.FromResult(Result.Success());

    public void Trigger()
    {
        logger.LogInformation("Wake word detectada (manual).");
        WakeWordDetected?.Invoke(this, configuration.Options.WakeWord);
    }
}
