using Microsoft.Extensions.DependencyInjection;
using Orion.Application.Abstractions;
using Orion.Application.Commands;
using Orion.Application.Dashboard;

namespace Orion.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra la capa de aplicación: reloj, contexto de usuario, motor de
    /// comandos (con descubrimiento automático de todos los <see cref="ICommand"/>)
    /// y el servicio de dashboard.
    /// </summary>
    public static IServiceCollection AddOrionApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IActiveUserContext, ActiveUserContext>();

        RegisterCommands(services);

        services.AddSingleton<ICommandRegistry, CommandRegistry>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IDashboardService, DashboardService>();

        return services;
    }

    /// <summary>
    /// Descubre por reflexión todas las implementaciones concretas de
    /// <see cref="ICommand"/> en este ensamblado y las registra como transitorias
    /// (una instancia nueva por ejecución, dentro de su propio ámbito de DI).
    /// Cada comando es resoluble por su tipo concreto y como <see cref="ICommand"/>.
    /// </summary>
    private static void RegisterCommands(IServiceCollection services)
    {
        var commandType = typeof(ICommand);
        var implementations = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && commandType.IsAssignableFrom(t));

        foreach (var implementation in implementations)
        {
            services.AddTransient(implementation);
            services.AddTransient(commandType, sp => sp.GetRequiredService(implementation));
        }
    }
}
