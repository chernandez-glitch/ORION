using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;

namespace Orion.Voice.Engine.Services;

/// <summary>
/// Micrófono activo. El nivel de entrada lo actualiza el pipeline durante la
/// grabación (medición real llegará con un proveedor STT que capture audio).
/// </summary>
internal sealed class MicrophoneService : IMicrophoneService
{
    private double _level;

    public int SelectedDeviceId { get; private set; } = -1;

    public Result SetDevice(int deviceId)
    {
        SelectedDeviceId = deviceId;
        return Result.Success();
    }

    public double GetLevel() => _level;

    public void SetLevel(double level) => _level = Math.Clamp(level, 0, 1);
}
