using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

internal sealed class VoiceConfigurationService : IVoiceConfigurationService
{
    public VoiceOptions Options { get; private set; } = new();

    public event EventHandler<VoiceOptions>? Changed;

    public void Update(VoiceOptions options)
    {
        Options = options.Clone();
        Changed?.Invoke(this, Options);
    }
}
