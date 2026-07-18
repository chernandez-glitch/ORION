namespace Orion.Shared.Modules;

/// <summary>
/// Estado operativo de un módulo de ORION, tal como se muestra en el dashboard.
/// </summary>
public enum ModuleStatus
{
    /// <summary>El módulo existe pero aún no se ha implementado/habilitado (fase de diseño).</summary>
    Disabled = 0,

    /// <summary>Configurado y listo para operar.</summary>
    Ready = 1,

    /// <summary>Operativo con capacidad reducida.</summary>
    Degraded = 2,

    /// <summary>No disponible por un fallo.</summary>
    Offline = 3
}
