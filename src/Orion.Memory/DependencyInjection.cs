using Microsoft.Extensions.DependencyInjection;
using Orion.Application.Memory;
using Orion.Shared.Modules;

namespace Orion.Memory;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el sistema de memoria. Depende de que la infraestructura haya
    /// registrado los repositorios y la unidad de trabajo (AddOrionInfrastructure).
    /// </summary>
    public static IServiceCollection AddOrionMemory(this IServiceCollection services)
    {
        services.AddScoped<IMemoryService, MemoryService>();
        services.AddSingleton<IModuleStatusProvider, MemoryStatusProvider>();
        return services;
    }
}
