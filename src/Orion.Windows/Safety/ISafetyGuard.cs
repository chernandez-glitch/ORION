using Orion.Shared.Results;

namespace Orion.Windows.Safety;

/// <summary>
/// Valida operaciones potencialmente peligrosas antes de ejecutarlas (borrar
/// rutas del sistema, matar procesos críticos, etc.). Devuelve un resultado
/// fallido para bloquear la acción.
/// </summary>
public interface ISafetyGuard
{
    Result EnsureSafeToDelete(string path);

    Result EnsureSafeToKill(string processName);
}
