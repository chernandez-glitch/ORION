using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Información de la pantalla.</summary>
public interface IScreenService
{
    Result<ScreenInfo> GetPrimaryScreen();
}
