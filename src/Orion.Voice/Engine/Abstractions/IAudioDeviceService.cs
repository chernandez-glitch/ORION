using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>Enumera los dispositivos de audio del sistema (entrada y salida).</summary>
public interface IAudioDeviceService
{
    Result<IReadOnlyList<AudioDevice>> GetInputDevices();

    Result<IReadOnlyList<AudioDevice>> GetOutputDevices();
}
