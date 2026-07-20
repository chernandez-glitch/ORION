using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Orion.Voice.Engine.Abstractions;
using Orion.Voice.Engine.Services;

namespace Orion.Voice.Engine;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el Voice Engine: configuración, dispositivos, micrófono, wake word,
    /// STT, TTS, historial, pipeline y la fachada. Todo singleton (vida de app).
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static IServiceCollection AddOrionVoiceEngine(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceConfigurationService, VoiceConfigurationService>();
        services.AddSingleton<IAudioDeviceService, AudioDeviceService>();
        services.AddSingleton<IMicrophoneService, MicrophoneService>();
        services.AddSingleton<IWakeWordService, WakeWordService>();
        services.AddSingleton<ISpeechRecognitionService, SpeechRecognitionService>();
        services.AddSingleton<ITextToSpeechService, TextToSpeechService>();
        services.AddSingleton<IVoiceHistory, VoiceHistory>();
        services.AddSingleton<IVoicePipeline, VoicePipeline>();
        services.AddSingleton<IVoiceEngine, VoiceEngine>();

        return services;
    }
}
