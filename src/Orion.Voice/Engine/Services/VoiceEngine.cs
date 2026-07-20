using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>Fachada que reúne todos los servicios del Voice Engine.</summary>
internal sealed class VoiceEngine(
    IVoicePipeline pipeline,
    IVoiceConfigurationService configuration,
    IAudioDeviceService devices,
    IMicrophoneService microphone,
    IVoiceHistory history,
    IWakeWordService wakeWord,
    ISpeechRecognitionService recognition,
    ITextToSpeechService synthesis) : IVoiceEngine
{
    public IVoicePipeline Pipeline { get; } = pipeline;

    public IVoiceConfigurationService Configuration { get; } = configuration;

    public IAudioDeviceService Devices { get; } = devices;

    public IMicrophoneService Microphone { get; } = microphone;

    public IVoiceHistory History { get; } = history;

    public IWakeWordService WakeWord { get; } = wakeWord;

    public ISpeechRecognitionService Recognition { get; } = recognition;

    public ITextToSpeechService Synthesis { get; } = synthesis;

    public VoiceState State => Pipeline.State;

    public Task<Result> StartAsync(CancellationToken cancellationToken = default) => Pipeline.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken = default) => Pipeline.StopAsync(cancellationToken);
}
