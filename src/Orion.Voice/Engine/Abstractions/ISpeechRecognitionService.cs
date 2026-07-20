using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>
/// Reconocimiento de voz a texto (STT). Intercambiable por configuración
/// (Simulado/Whisper local/Whisper API/Azure/Windows). En esta fase solo el
/// simulado está disponible.
/// </summary>
public interface ISpeechRecognitionService
{
    SttProvider Provider { get; }

    bool IsAvailable { get; }

    /// <summary>
    /// Reconoce el habla. Con el proveedor simulado, <paramref name="simulatedInput"/>
    /// es el texto a devolver; los proveedores reales lo ignoran y usan el micrófono.
    /// </summary>
    Task<Result<SpeechResult>> RecognizeAsync(string simulatedInput, CancellationToken cancellationToken = default);
}
