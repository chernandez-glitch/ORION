using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Application.Dashboard;
using Orion.Domain.Automations;
using Orion.Domain.Common;
using Orion.Domain.Conversations;
using Orion.Domain.History;
using Orion.Domain.Preferences;
using Orion.Domain.Projects;
using Orion.Domain.Routes;
using Orion.Domain.Users;
using Orion.Infrastructure.Diagnostics;
using Orion.Infrastructure.Logging;
using Orion.Infrastructure.Persistence;
using Orion.Infrastructure.Persistence.Repositories;
using Serilog;

namespace Orion.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra la infraestructura: DbContext SQLite, unidad de trabajo,
    /// repositorios y métricas del sistema.
    /// </summary>
    public static IServiceCollection AddOrionInfrastructure(this IServiceCollection services, string? databasePath = null)
    {
        var dbPath = databasePath ?? DefaultDatabasePath();
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        services.AddDbContext<OrionDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrionDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPreferenceRepository, PreferenceRepository>();
        services.AddScoped<IFavoriteProjectRepository, FavoriteProjectRepository>();
        services.AddScoped<IFavoriteRouteRepository, FavoriteRouteRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<ICommandHistoryRepository, CommandHistoryRepository>();
        services.AddScoped<IAutomationRepository, AutomationRepository>();

        services.AddSingleton<ISystemMetrics, SystemMetrics>();

        return services;
    }

    /// <summary>Configura Serilog como proveedor de logging de la aplicación.</summary>
    public static IServiceCollection AddOrionLogging(this IServiceCollection services, string? logDirectory = null)
    {
        var logger = SerilogConfigurator.Create(logDirectory ?? SerilogConfigurator.DefaultLogDirectory());

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(logger, dispose: true);
        });

        return services;
    }

    /// <summary>Aplica las migraciones pendientes, creando la base si no existe.</summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<OrionDbContext>();
        await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }

    public static string DefaultDatabasePath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "OrionAI", "orion.db");
    }
}
