using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Voice.Engine;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel del Voice Center: estado del pipeline, control (escuchar / simular
/// "Orion" / enviar habla), dispositivos, configuración e historial. El estado y
/// el nivel del micrófono se refrescan por sondeo (Poll) desde la página.
/// </summary>
public sealed partial class VoiceCenterViewModel : ObservableObject
{
    private readonly IVoiceEngine _engine;

    public VoiceCenterViewModel(IVoiceEngine engine)
    {
        _engine = engine;

        var options = engine.Configuration.Options;
        _wakeWord = options.WakeWord;
        _engineIndex = (int)options.WakeWordEngine;
        _sensitivity = options.Sensitivity;
        _sttIndex = (int)options.Stt;
        _ttsIndex = (int)options.Tts;
        _language = options.Language;

        RefreshDevices();
    }

    public ObservableCollection<AudioDevice> InputDevices { get; } = [];

    public ObservableCollection<AudioDevice> OutputDevices { get; } = [];

    public ObservableCollection<VoiceInteraction> History { get; } = [];

    public IReadOnlyList<string> WakeEngines { get; } = Enum.GetNames<WakeWordEngine>();

    public IReadOnlyList<string> SttProviders { get; } = Enum.GetNames<SttProvider>();

    public IReadOnlyList<string> TtsProviders { get; } = Enum.GetNames<TtsProvider>();

    [ObservableProperty]
    private string _stateText = "Inactivo";

    [ObservableProperty]
    private double _micLevel;

    [ObservableProperty]
    private bool _isListening;

    [ObservableProperty]
    private string _simulatedText = string.Empty;

    [ObservableProperty]
    private string _lastResult = string.Empty;

    [ObservableProperty]
    private long _responseMs;

    [ObservableProperty]
    private string _wakeWord;

    [ObservableProperty]
    private int _engineIndex;

    [ObservableProperty]
    private double _sensitivity;

    [ObservableProperty]
    private int _sttIndex;

    [ObservableProperty]
    private int _ttsIndex;

    [ObservableProperty]
    private string _language;

    [ObservableProperty]
    private int _selectedInputIndex = -1;

    [ObservableProperty]
    private string _configStatus = string.Empty;

    /// <summary>Refresca estado y nivel del micrófono (llamado por un timer de la página).</summary>
    public void Poll()
    {
        StateText = Translate(_engine.State);
        MicLevel = _engine.Microphone.GetLevel() * 100;
    }

    [RelayCommand]
    private async Task StartListeningAsync()
    {
        var result = await _engine.StartAsync().ConfigureAwait(true);
        IsListening = result.IsSuccess;
        if (result.IsFailure)
        {
            LastResult = result.Error.Message;
        }
    }

    [RelayCommand]
    private async Task StopListeningAsync()
    {
        await _engine.StopAsync().ConfigureAwait(true);
        IsListening = false;
    }

    [RelayCommand]
    private void TriggerWake() => _engine.Pipeline.TriggerWakeWord();

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(SimulatedText))
        {
            return;
        }

        var result = await _engine.Pipeline.SubmitSpeechAsync(SimulatedText).ConfigureAwait(true);
        if (result.IsSuccess)
        {
            LastResult = result.Value.ResultText;
            ResponseMs = result.Value.DurationMs;
            History.Insert(0, result.Value);
            SimulatedText = string.Empty;
        }
        else
        {
            LastResult = result.Error.Message;
        }
    }

    [RelayCommand]
    private void RefreshDevices()
    {
        InputDevices.Clear();
        var inputs = _engine.Devices.GetInputDevices();
        if (inputs.IsSuccess)
        {
            foreach (var device in inputs.Value)
            {
                InputDevices.Add(device);
            }
        }

        OutputDevices.Clear();
        var outputs = _engine.Devices.GetOutputDevices();
        if (outputs.IsSuccess)
        {
            foreach (var device in outputs.Value)
            {
                OutputDevices.Add(device);
            }
        }
    }

    [RelayCommand]
    private void SaveConfig()
    {
        var options = new VoiceOptions
        {
            WakeWord = WakeWord,
            WakeWordEngine = (WakeWordEngine)EngineIndex,
            Sensitivity = Sensitivity,
            Stt = (SttProvider)SttIndex,
            Tts = (TtsProvider)TtsIndex,
            Language = Language,
            MicrophoneId = SelectedInputIndex >= 0 && SelectedInputIndex < InputDevices.Count ? InputDevices[SelectedInputIndex].Id : -1
        };

        _engine.Configuration.Update(options);
        if (options.MicrophoneId >= 0)
        {
            _engine.Microphone.SetDevice(options.MicrophoneId);
        }

        ConfigStatus = "Configuración de voz guardada.";
    }

    private static string Translate(VoiceState state) => state switch
    {
        VoiceState.Listening => "Escuchando",
        VoiceState.Recording => "Grabando",
        VoiceState.Processing => "Procesando",
        VoiceState.Speaking => "Hablando",
        VoiceState.Error => "Error",
        _ => "Inactivo"
    };
}
