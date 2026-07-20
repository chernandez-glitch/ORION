using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// TTS seleccionable por configuración. En esta fase no vocaliza (None): registra
/// el texto. Piper/Windows/Azure/ElevenLabs/OpenAI quedan preparados como interfaces.
/// </summary>
internal sealed class TextToSpeechService(IVoiceConfigurationService configuration, ILogger<TextToSpeechService> logger) : ITextToSpeechService
{
    public TtsProvider Provider => configuration.Options.Tts;

    public bool IsAvailable => Provider == TtsProvider.None;

    public Task<Result> SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (Provider != TtsProvider.None)
        {
            return Task.FromResult(Result.Failure(Error.Failure("Voice.TtsUnavailable",
                $"El proveedor TTS '{Provider}' aún no está disponible en esta fase.")));
        }

        logger.LogInformation("TTS (sin voz) — texto: {Text}", text);
        return Task.FromResult(Result.Success());
    }
}
