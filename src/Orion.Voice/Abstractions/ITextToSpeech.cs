using Orion.Shared.Results;

namespace Orion.Voice.Abstractions;

/// <summary>Síntesis de texto a voz (TTS). Implementación en Fase 3.</summary>
public interface ITextToSpeech
{
    Task<Result> SpeakAsync(string text, string voice, CancellationToken cancellationToken = default);

    Task<Result<Stream>> SynthesizeAsync(string text, string voice, CancellationToken cancellationToken = default);
}
