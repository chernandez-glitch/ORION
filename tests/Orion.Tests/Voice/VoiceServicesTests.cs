using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Orion.Voice.Engine;
using Orion.Voice.Engine.Services;
using Xunit;

namespace Orion.Tests.Voice;

/// <summary>
/// Pruebas de los servicios de soporte del Voice Engine: configuración, historial,
/// STT simulado, wake word y micrófono.
/// </summary>
public sealed class VoiceServicesTests
{
    [Fact]
    public void Configuration_Update_PersistsCloneAndRaisesChanged()
    {
        var config = new VoiceConfigurationService();
        VoiceOptions? changed = null;
        config.Changed += (_, o) => changed = o;

        config.Update(new VoiceOptions { WakeWord = "Nova", Language = "en-US" });

        config.Options.WakeWord.Should().Be("Nova");
        config.Options.Language.Should().Be("en-US");
        changed.Should().NotBeNull();
    }

    [Fact]
    public void VoiceOptions_Clone_IsIndependentCopy()
    {
        var original = new VoiceOptions { WakeWord = "Orion", Sensitivity = 0.7 };

        var clone = original.Clone();
        clone.WakeWord = "Otro";

        original.WakeWord.Should().Be("Orion");
        clone.Sensitivity.Should().Be(0.7);
    }

    [Fact]
    public void History_GetRecent_ReturnsMostRecentFirst()
    {
        var history = new VoiceHistory();
        history.Add(Interaction("uno"));
        history.Add(Interaction("dos"));
        history.Add(Interaction("tres"));

        var recent = history.GetRecent(2);

        recent.Should().HaveCount(2);
        recent[0].RecognizedText.Should().Be("tres");
        recent[1].RecognizedText.Should().Be("dos");
    }

    [Fact]
    public async Task SpeechRecognition_Simulated_EchoesTrimmedInputWithProvider()
    {
        var config = new VoiceConfigurationService();
        var stt = new SpeechRecognitionService(config, NullLogger<SpeechRecognitionService>.Instance);

        var result = await stt.RecognizeAsync("  hola  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be("hola");
        result.Value.Provider.Should().Be("Simulado");
    }

    [Fact]
    public async Task SpeechRecognition_NonSimulatedProvider_IsUnavailable()
    {
        var config = new VoiceConfigurationService();
        config.Update(new VoiceOptions { Stt = SttProvider.AzureSpeech });
        var stt = new SpeechRecognitionService(config, NullLogger<SpeechRecognitionService>.Instance);

        stt.IsAvailable.Should().BeFalse();
        (await stt.RecognizeAsync("x")).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void WakeWord_ManualIsAvailable_OtherEnginesAreNot()
    {
        var config = new VoiceConfigurationService();
        var wake = new WakeWordService(config, NullLogger<WakeWordService>.Instance);
        wake.IsAvailable.Should().BeTrue();

        config.Update(new VoiceOptions { WakeWordEngine = WakeWordEngine.OpenWakeWord });
        wake.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void WakeWord_Trigger_RaisesDetectedWithConfiguredWord()
    {
        var config = new VoiceConfigurationService();
        config.Update(new VoiceOptions { WakeWord = "Orion" });
        var wake = new WakeWordService(config, NullLogger<WakeWordService>.Instance);
        string? detected = null;
        wake.WakeWordDetected += (_, w) => detected = w;

        wake.Trigger();

        detected.Should().Be("Orion");
    }

    [Fact]
    public void Microphone_SetLevel_ClampsToUnitRange()
    {
        var mic = new MicrophoneService();

        mic.SetLevel(2.5);
        mic.GetLevel().Should().Be(1);

        mic.SetLevel(-1);
        mic.GetLevel().Should().Be(0);
    }

    [Fact]
    public void Microphone_SetDevice_UpdatesSelectedId()
    {
        var mic = new MicrophoneService();

        mic.SetDevice(3).IsSuccess.Should().BeTrue();

        mic.SelectedDeviceId.Should().Be(3);
    }

    private static VoiceInteraction Interaction(string text) =>
        new(Guid.CreateVersion7(), text, "Simulado", DateTime.UtcNow, 1, "ok", true);
}
