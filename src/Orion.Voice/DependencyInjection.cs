using Microsoft.Extensions.DependencyInjection;
using Orion.Shared.Modules;

namespace Orion.Voice;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el módulo de voz. En esta fase solo reporta estado al dashboard;
    /// no hay motores de voz concretos todavía.
    /// </summary>
    public static IServiceCollection AddOrionVoice(this IServiceCollection services)
    {
        services.AddSingleton<IModuleStatusProvider, VoiceStatusProvider>();
        return services;
    }
}
