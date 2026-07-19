using Microsoft.Extensions.DependencyInjection;
using Orion.Application.Abstractions;
using Orion.Application.Commands;
using Orion.Application.Dashboard;

namespace Orion.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra la capa de aplicación: reloj, contexto de usuario, el motor de
    /// comandos completo (registro automático de comandos, pipeline, executor,
    /// registry, validador, autorizador, historial y parser) y el dashboard.
    /// </summary>
    public static IServiceCollection AddOrionApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IActiveUserContext, ActiveUserContext>();

        RegisterCommands(services);

        // Motor de comandos.
        services.AddSingleton<ICommandRegistry, CommandRegistry>();
        services.AddSingleton<ICommandParser, DefaultCommandParser>();
        services.AddSingleton<ICommandValidator, CommandValidator>();
        services.AddSingleton<ICommandAuthorizer, DefaultCommandAuthorizer>();
        services.AddScoped<ICommandHistory, MemoryCommandHistory>();
        services.AddScoped<ICommandPipeline, CommandPipeline>();
        services.AddSingleton<ICommandExecutor, CommandExecutor>();

        services.AddSingleton<IDashboardService, DashboardService>();

        return services;
    }

    /// <summary>
    /// Descubre por reflexión todas las clases concretas que heredan de
    /// <see cref="CommandBase"/> y las registra como transitorias (una instancia
    /// por ejecución, dentro de su propio ámbito de DI) y como <see cref="ICommand"/>.
    /// Agregar un comando nuevo no requiere tocar la configuración de DI.
    /// </summary>
    private static void RegisterCommands(IServiceCollection services)
    {
        var commandBase = typeof(CommandBase);
        var implementations = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && commandBase.IsAssignableFrom(t));

        foreach (var implementation in implementations)
        {
            services.AddTransient(implementation);
            services.AddTransient(typeof(ICommand), sp => sp.GetRequiredService(implementation));
        }
    }
}
