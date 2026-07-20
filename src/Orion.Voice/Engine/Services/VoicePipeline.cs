using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Application.Commands;
using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// Pipeline de voz: micrófono → wake word → grabación → STT → parser → resultado.
/// El paso "parser" usa el <see cref="ICommandParser"/> del Command Engine SIN
/// ejecutar el comando (solo se muestra el resultado). Ni IA ni ejecución.
/// </summary>
internal sealed class VoicePipeline(
    IVoiceConfigurationService configuration,
    ISpeechRecognitionService recognition,
    IWakeWordService wakeWord,
    IMicrophoneService microphone,
    IVoiceHistory history,
    ICommandParser parser,
    ICommandRegistry registry,
    ILogger<VoicePipeline> logger) : IVoicePipeline
{
    private bool _listening;

    public VoiceState State { get; private set; } = VoiceState.Idle;

    public event EventHandler<VoiceState>? StateChanged;

    public event EventHandler<VoiceInteraction>? ResultReady;

    public async Task<Result> StartAsync(CancellationToken cancellationToken = default)
    {
        var options = configuration.Options;
        var start = await wakeWord.StartAsync(options.WakeWord, options.Sensitivity, cancellationToken).ConfigureAwait(false);
        if (start.IsFailure)
        {
            SetState(VoiceState.Error);
            return start;
        }

        _listening = true;
        SetState(VoiceState.Listening);
        logger.LogInformation("Pipeline de voz escuchando (wake word '{WakeWord}').", options.WakeWord);
        return Result.Success();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _listening = false;
        await wakeWord.StopAsync(cancellationToken).ConfigureAwait(false);
        microphone.SetLevel(0);
        SetState(VoiceState.Idle);
    }

    public void TriggerWakeWord()
    {
        wakeWord.Trigger();
        microphone.SetLevel(0.6);
        SetState(VoiceState.Recording);
    }

    public async Task<Result<VoiceInteraction>> SubmitSpeechAsync(string recognizedText, CancellationToken cancellationToken = default)
    {
        SetState(VoiceState.Processing);
        var stopwatch = Stopwatch.StartNew();

        var stt = await recognition.RecognizeAsync(recognizedText, cancellationToken).ConfigureAwait(false);
        if (stt.IsFailure)
        {
            SetState(_listening ? VoiceState.Listening : VoiceState.Idle);
            return Result.Failure<VoiceInteraction>(stt.Error);
        }

        var text = stt.Value.Text;
        var parse = parser.Parse(text, registry);
        var resultText = parse.Success
            ? $"Comando reconocido: '{parse.CommandKey}' (no ejecutado en esta fase)."
            : $"No se reconoció un comando: {parse.Error}";

        stopwatch.Stop();

        var interaction = new VoiceInteraction(
            Guid.CreateVersion7(), text, stt.Value.Provider, DateTime.UtcNow,
            stopwatch.ElapsedMilliseconds, resultText, parse.Success);

        history.Add(interaction);
        microphone.SetLevel(0);
        ResultReady?.Invoke(this, interaction);
        SetState(_listening ? VoiceState.Listening : VoiceState.Idle);

        logger.LogInformation("Voz procesada: '{Text}' → {Result}", text, resultText);
        return Result.Success(interaction);
    }

    private void SetState(VoiceState state)
    {
        State = state;
        StateChanged?.Invoke(this, state);
    }
}
