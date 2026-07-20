using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// Historial de voz en memoria (por sesión). La persistencia podrá enrutarse al
/// Memory Engine más adelante sin cambiar esta interfaz.
/// </summary>
internal sealed class VoiceHistory : IVoiceHistory
{
    private readonly List<VoiceInteraction> _interactions = [];
    private readonly Lock _sync = new();

    public event EventHandler<VoiceInteraction>? Added;

    public void Add(VoiceInteraction interaction)
    {
        lock (_sync)
        {
            _interactions.Add(interaction);
        }

        Added?.Invoke(this, interaction);
    }

    public IReadOnlyList<VoiceInteraction> GetRecent(int take)
    {
        lock (_sync)
        {
            return _interactions.AsEnumerable().Reverse().Take(take).ToArray();
        }
    }
}
