using Microsoft.Extensions.DependencyInjection;

namespace Orion.Plugins;

/// <summary>
/// Contrato de un plugin de ORION. Cada integración (SAP, SQL, GitHub, Docker,
/// Power BI...) es un plugin independiente que se descubre y carga en runtime.
/// Un plugin aporta sus propios servicios y comandos al contenedor de DI.
/// </summary>
public interface IOrionPlugin
{
    PluginMetadata Metadata { get; }

    /// <summary>Registra los servicios y comandos que el plugin aporta.</summary>
    void Register(IServiceCollection services);
}
