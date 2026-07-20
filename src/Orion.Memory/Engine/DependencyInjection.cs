using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Persistence;
using Orion.Memory.Engine.Services;

namespace Orion.Memory.Engine;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el Memory Engine: DbContext SQLite propio, repositorio genérico y
    /// todos los servicios de memoria (sesión, conversación, proyecto, historial,
    /// favoritos, etiquetas, búsqueda, contexto, exportación) + la fachada.
    /// </summary>
    public static IServiceCollection AddOrionMemoryEngine(this IServiceCollection services, string? databasePath = null)
    {
        var dbPath = databasePath ?? DefaultMemoryDatabasePath();
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        services.AddDbContext<MemoryDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

        services.AddSingleton<IMemorySession, MemorySession>();
        services.AddScoped(typeof(IMemoryRepository<>), typeof(EfMemoryRepository<>));
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IProjectMemoryService, ProjectMemoryService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISearchMemoryService, SearchMemoryService>();
        services.AddScoped<IContextService, ContextService>();
        services.AddScoped<IMemoryExportService, JsonCsvMemoryExportService>();
        services.AddScoped<IMemoryEngine, MemoryEngine>();

        return services;
    }

    /// <summary>Aplica las migraciones de la base de memoria (la crea si no existe).</summary>
    public static async Task InitializeMemoryDatabaseAsync(this IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MemoryDbContext>();
        await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }

    public static string DefaultMemoryDatabasePath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "OrionAI", "memory.db");
    }
}
