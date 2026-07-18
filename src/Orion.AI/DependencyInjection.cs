using Microsoft.Extensions.DependencyInjection;
using Orion.Shared.Modules;

namespace Orion.AI;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el módulo de IA. En esta fase solo aporta su estado al
    /// dashboard: no hay proveedores concretos todavía (ver <see cref="AIProviderKind"/>).
    /// </summary>
    public static IServiceCollection AddOrionAI(this IServiceCollection services)
    {
        services.AddSingleton<IModuleStatusProvider, AIStatusProvider>();
        return services;
    }
}
