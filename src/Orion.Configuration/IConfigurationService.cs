using Orion.Configuration.Models;
using Orion.Shared.Results;

namespace Orion.Configuration;

/// <summary>
/// Carga y persiste la configuración de ORION. Mantiene una copia en memoria
/// (<see cref="Current"/>) y notifica cambios para que la UI reaccione.
/// </summary>
public interface IConfigurationService
{
    /// <summary>Configuración vigente en memoria.</summary>
    OrionSettings Current { get; }

    /// <summary>Se dispara tras guardar cambios exitosamente.</summary>
    event EventHandler<OrionSettings>? Changed;

    Task<Result<OrionSettings>> LoadAsync(CancellationToken cancellationToken = default);

    Task<Result> SaveAsync(OrionSettings settings, CancellationToken cancellationToken = default);
}
