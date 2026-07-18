using Orion.Shared.Modules;

namespace Orion.Voice;

internal sealed class VoiceStatusProvider : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus() =>
        new("Micrófono", ModuleStatus.Disabled, "Contratos de voz definidos (STT/TTS/WakeWord). Implementación en Fase 3.");
}
