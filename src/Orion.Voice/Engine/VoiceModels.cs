namespace Orion.Voice.Engine;

/// <summary>Estado del pipeline de voz.</summary>
public enum VoiceState
{
    Idle = 0,
    Listening = 1,
    Recording = 2,
    Processing = 3,
    Speaking = 4,
    Error = 5
}

/// <summary>Motor de detección de palabra de activación.</summary>
public enum WakeWordEngine
{
    Manual = 0,
    OpenWakeWord = 1,
    Porcupine = 2,
    Custom = 3
}

/// <summary>Proveedor de reconocimiento de voz (STT).</summary>
public enum SttProvider
{
    Simulated = 0,
    WhisperLocal = 1,
    WhisperApi = 2,
    AzureSpeech = 3,
    WindowsSpeech = 4
}

/// <summary>Proveedor de síntesis de voz (TTS).</summary>
public enum TtsProvider
{
    None = 0,
    Piper = 1,
    WindowsSpeech = 2,
    AzureSpeech = 3,
    ElevenLabs = 4,
    OpenAI = 5
}

/// <summary>Dispositivo de audio (entrada o salida).</summary>
public sealed record AudioDevice(int Id, string Name, bool IsInput);

/// <summary>Resultado del reconocimiento de voz.</summary>
public sealed record SpeechResult(string Text, double Confidence, long DurationMs, string Provider);

/// <summary>Entrada del historial de voz.</summary>
public sealed record VoiceInteraction(
    Guid Id,
    string RecognizedText,
    string Provider,
    DateTime OccurredOnUtc,
    long DurationMs,
    string ResultText,
    bool CommandRecognized);

/// <summary>Opciones configurables del Voice Engine.</summary>
public sealed class VoiceOptions
{
    public int MicrophoneId { get; set; } = -1;

    public int SpeakerId { get; set; } = -1;

    public string WakeWord { get; set; } = "Orion";

    public WakeWordEngine WakeWordEngine { get; set; } = WakeWordEngine.Manual;

    public SttProvider Stt { get; set; } = SttProvider.Simulated;

    public TtsProvider Tts { get; set; } = TtsProvider.None;

    /// <summary>Sensibilidad de la palabra de activación (0.0–1.0).</summary>
    public double Sensitivity { get; set; } = 0.5;

    public string Language { get; set; } = "es-HN";

    public VoiceOptions Clone() => (VoiceOptions)MemberwiseClone();
}
