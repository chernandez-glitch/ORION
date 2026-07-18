using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Shared.Modules;

namespace Orion.Configuration;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el servicio de configuración respaldado por JSON. Si no se
    /// indica ruta, se usa <c>%AppData%\OrionAI\settings.json</c>.
    /// </summary>
    public static IServiceCollection AddOrionConfiguration(this IServiceCollection services, string? filePath = null)
    {
        var resolvedPath = filePath ?? DefaultSettingsPath();

        services.AddSingleton<IConfigurationService>(sp =>
            new JsonConfigurationService(resolvedPath, sp.GetRequiredService<ILogger<JsonConfigurationService>>()));

        services.AddSingleton<IModuleStatusProvider, ConfigurationStatusProvider>();
        return services;
    }

    public static string DefaultSettingsPath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "OrionAI", "settings.json");
    }
}
