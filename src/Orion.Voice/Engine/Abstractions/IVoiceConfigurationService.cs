namespace Orion.Voice.Engine.Abstractions;

/// <summary>Configuración del Voice Engine (micrófono, altavoz, proveedores, wake word, sensibilidad, idioma).</summary>
public interface IVoiceConfigurationService
{
    VoiceOptions Options { get; }

    event EventHandler<VoiceOptions>? Changed;

    void Update(VoiceOptions options);
}
