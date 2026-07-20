using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// STT seleccionable por configuración. En esta fase solo el proveedor Simulado
/// está disponible (devuelve el texto provisto); Whisper/Azure/Windows quedan
/// preparados como interfaces.
/// </summary>
internal sealed class SpeechRecognitionService(IVoiceConfigurationService configuration, ILogger<SpeechRecognitionService> logger) : ISpeechRecognitionService
{
    public SttProvider Provider => configuration.Options.Stt;

    public bool IsAvailable => Provider == SttProvider.Simulated;

    public Task<Result<SpeechResult>> RecognizeAsync(string simulatedInput, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return Task.FromResult(Result.Failure<SpeechResult>(Error.Failure("Voice.SttUnavailable",
                $"El proveedor STT '{Provider}' aún no está disponible en esta fase.")));
        }

        var stopwatch = Stopwatch.StartNew();
        var text = (simulatedInput ?? string.Empty).Trim();
        stopwatch.Stop();

        logger.LogInformation("STT (Simulado) reconoció: {Text}", text);
        return Task.FromResult(Result.Success(new SpeechResult(text, 1.0, stopwatch.ElapsedMilliseconds, "Simulado")));
    }
}
