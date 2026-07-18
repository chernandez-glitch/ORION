using Orion.Shared.Results;
using Orion.Voice.Models;

namespace Orion.Voice.Abstractions;

/// <summary>Reconocimiento de voz a texto (STT). Implementación en Fase 3.</summary>
public interface ISpeechToText
{
    /// <summary>Transcribe un flujo de audio PCM.</summary>
    Task<Result<TranscriptionResult>> TranscribeAsync(Stream audio, string language, CancellationToken cancellationToken = default);

    /// <summary>Escucha continua: emite transcripciones a medida que se detecta habla.</summary>
    IAsyncEnumerable<TranscriptionResult> StreamAsync(string language, CancellationToken cancellationToken = default);
}
