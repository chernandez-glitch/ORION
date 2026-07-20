namespace Orion.Voice.Engine.Abstractions;

/// <summary>Historial de interacciones de voz (texto reconocido, tiempo, proveedor, resultado).</summary>
public interface IVoiceHistory
{
    event EventHandler<VoiceInteraction>? Added;

    void Add(VoiceInteraction interaction);

    IReadOnlyList<VoiceInteraction> GetRecent(int take);
}
