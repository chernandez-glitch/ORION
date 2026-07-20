using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>
/// Síntesis de texto a voz (TTS). Intercambiable por configuración
/// (Piper/Windows/Azure/ElevenLabs/OpenAI). En esta fase no vocaliza (None).
/// </summary>
public interface ITextToSpeechService
{
    TtsProvider Provider { get; }

    bool IsAvailable { get; }

    Task<Result> SpeakAsync(string text, CancellationToken cancellationToken = default);
}
