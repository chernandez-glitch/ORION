using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>Fachada del Voice Engine: reúne pipeline, configuración, dispositivos, micrófono e historial.</summary>
public interface IVoiceEngine
{
    IVoicePipeline Pipeline { get; }

    IVoiceConfigurationService Configuration { get; }

    IAudioDeviceService Devices { get; }

    IMicrophoneService Microphone { get; }

    IVoiceHistory History { get; }

    IWakeWordService WakeWord { get; }

    ISpeechRecognitionService Recognition { get; }

    ITextToSpeechService Synthesis { get; }

    VoiceState State { get; }

    Task<Result> StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
