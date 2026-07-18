using Microsoft.Extensions.DependencyInjection;

namespace Orion.Installer;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el módulo de instalación/actualización. En esta fase usa un
    /// servicio de diseño que reporta "al día".
    /// </summary>
    public static IServiceCollection AddOrionInstaller(this IServiceCollection services)
    {
        services.AddSingleton<IUpdateService, PhaseZeroUpdateService>();
        return services;
    }
}
