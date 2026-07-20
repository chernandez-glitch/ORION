using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Orion.Application.Commands;
using Orion.Tests.Application;
using Orion.Voice.Engine;
using Orion.Voice.Engine.Services;
using Xunit;

namespace Orion.Tests.Voice;

/// <summary>
/// Pruebas del pipeline de voz: wake word → grabación → STT (simulado) → parser →
/// resultado. Verifican explícitamente que el pipeline NO ejecuta comandos: solo
/// reconoce y muestra el resultado, dejando la costura lista para la IA.
/// </summary>
public sealed class VoicePipelineTests
{
    private readonly VoiceConfigurationService _config = new();
    private readonly VoiceHistory _history = new();
    private readonly MicrophoneService _microphone = new();
    private readonly FakeCommandRegistry _registry = new(TestCommands.WithRequiredParam("apps.open"));
    private readonly DefaultCommandParser _parser = new();

    private VoicePipeline CreatePipeline() => new(
        _config,
        new SpeechRecognitionService(_config, NullLogger<SpeechRecognitionService>.Instance),
        new WakeWordService(_config, NullLogger<WakeWordService>.Instance),
        _microphone,
        _history,
        _parser,
        _registry,
        NullLogger<VoicePipeline>.Instance);

    [Fact]
    public async Task StartAsync_WithManualEngine_TransitionsToListening()
    {
        var pipeline = CreatePipeline();

        var result = await pipeline.StartAsync();

        result.IsSuccess.Should().BeTrue();
        pipeline.State.Should().Be(VoiceState.Listening);
    }

    [Fact]
    public async Task StartAsync_WithUnavailableEngine_FailsAndEntersErrorState()
    {
        _config.Update(new VoiceOptions { WakeWordEngine = WakeWordEngine.Porcupine });
        var pipeline = CreatePipeline();

        var result = await pipeline.StartAsync();

        result.IsFailure.Should().BeTrue();
        pipeline.State.Should().Be(VoiceState.Error);
    }

    [Fact]
    public void TriggerWakeWord_EntersRecordingAndRaisesMicLevel()
    {
        var pipeline = CreatePipeline();

        pipeline.TriggerWakeWord();

        pipeline.State.Should().Be(VoiceState.Recording);
        _microphone.GetLevel().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SubmitSpeechAsync_WithRecognizedCommand_ReportsRecognizedButNotExecuted()
    {
        var pipeline = CreatePipeline();
        await pipeline.StartAsync();

        var result = await pipeline.SubmitSpeechAsync("alias notepad");

        result.IsSuccess.Should().BeTrue();
        result.Value.CommandRecognized.Should().BeTrue();
        result.Value.RecognizedText.Should().Be("alias notepad");
        result.Value.ResultText.Should().Contain("no ejecutado");
        result.Value.Provider.Should().Be("Simulado");
    }

    [Fact]
    public async Task SubmitSpeechAsync_WithUnknownCommand_ReportsNotRecognized()
    {
        var pipeline = CreatePipeline();

        var result = await pipeline.SubmitSpeechAsync("comando-inexistente");

        result.IsSuccess.Should().BeTrue();
        result.Value.CommandRecognized.Should().BeFalse();
        result.Value.ResultText.Should().Contain("No se reconoció");
    }

    [Fact]
    public async Task SubmitSpeechAsync_RecordsHistoryAndRaisesResultReady()
    {
        var pipeline = CreatePipeline();
        VoiceInteraction? raised = null;
        pipeline.ResultReady += (_, i) => raised = i;

        await pipeline.SubmitSpeechAsync("alias notepad");

        raised.Should().NotBeNull();
        _history.GetRecent(10).Should().ContainSingle()
            .Which.RecognizedText.Should().Be("alias notepad");
    }

    [Fact]
    public async Task SubmitSpeechAsync_ResetsMicLevelAfterProcessing()
    {
        var pipeline = CreatePipeline();
        pipeline.TriggerWakeWord();

        await pipeline.SubmitSpeechAsync("alias notepad");

        _microphone.GetLevel().Should().Be(0);
    }

    [Fact]
    public async Task SubmitSpeechAsync_WithUnavailableStt_ReturnsFailure()
    {
        _config.Update(new VoiceOptions { Stt = SttProvider.WhisperLocal });
        var pipeline = CreatePipeline();

        var result = await pipeline.SubmitSpeechAsync("alias notepad");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Voice.SttUnavailable");
    }

    [Fact]
    public async Task FullFlow_Start_Trigger_Submit_ReturnsToListening()
    {
        var pipeline = CreatePipeline();

        await pipeline.StartAsync();
        pipeline.State.Should().Be(VoiceState.Listening);

        pipeline.TriggerWakeWord();
        pipeline.State.Should().Be(VoiceState.Recording);

        await pipeline.SubmitSpeechAsync("alias notepad");
        pipeline.State.Should().Be(VoiceState.Listening);
    }
}
