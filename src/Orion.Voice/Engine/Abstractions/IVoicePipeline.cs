using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>
/// Pipeline de voz: micrófono → wake word → grabación → STT → parser → resultado.
/// En esta fase NO ejecuta comandos ni usa IA: solo llega hasta parsear el texto
/// reconocido y mostrar el resultado.
/// </summary>
public interface IVoicePipeline
{
    VoiceState State { get; }

    event EventHandler<VoiceState>? StateChanged;

    event EventHandler<VoiceInteraction>? ResultReady;

    /// <summary>Comienza a escuchar (a la espera de la palabra de activación).</summary>
    Task<Result> StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>Simula la detección de la palabra de activación (motor Manual).</summary>
    void TriggerWakeWord();

    /// <summary>
    /// Procesa el habla: STT → parser (Command Engine, sin ejecutar) → resultado
    /// → historial. Devuelve la interacción registrada.
    /// </summary>
    Task<Result<VoiceInteraction>> SubmitSpeechAsync(string recognizedText, CancellationToken cancellationToken = default);
}
