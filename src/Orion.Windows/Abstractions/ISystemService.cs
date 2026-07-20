using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Acciones de sistema y energía.</summary>
public interface ISystemService
{
    Result Shutdown(TimeSpan delay);

    Result Restart(TimeSpan delay);

    Result Suspend();

    Result Lock();

    Result LogOff();

    Result<SystemInfo> GetSystemInfo();
}
