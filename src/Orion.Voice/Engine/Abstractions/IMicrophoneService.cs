using Orion.Shared.Results;

namespace Orion.Voice.Engine.Abstractions;

/// <summary>Micrófono activo: selección y nivel de entrada.</summary>
public interface IMicrophoneService
{
    int SelectedDeviceId { get; }

    Result SetDevice(int deviceId);

    /// <summary>Nivel de entrada actual (0.0–1.0).</summary>
    double GetLevel();

    /// <summary>Actualiza el nivel (lo usa el pipeline durante la grabación).</summary>
    void SetLevel(double level);
}
