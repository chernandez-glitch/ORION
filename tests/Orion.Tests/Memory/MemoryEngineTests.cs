using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Orion.Memory.Engine;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Xunit;

namespace Orion.Tests.Memory;

/// <summary>Pruebas del Memory Engine sobre una base SQLite temporal real.</summary>
public sealed class MemoryEngineTests : IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"orion-mem-{Guid.NewGuid():N}.db");
    private ServiceProvider _provider = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOrionMemoryEngine(_dbPath);
        _provider = services.BuildServiceProvider();
        await _provider.InitializeMemoryDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _provider.DisposeAsync();
        SqliteConnection.ClearAllPools();

        foreach (var file in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch (IOException)
            {
                // Limpieza best-effort: el archivo temporal se recogerá luego.
            }
        }
    }

    private IServiceScope Scope() => _provider.CreateScope();

    [Fact]
    public async Task Sessions_StartAndCount()
    {
        using (var scope = Scope())
        {
            var sessions = scope.ServiceProvider.GetRequiredService<ISessionService>();
            (await sessions.StartSessionAsync()).IsSuccess.Should().BeTrue();
            (await sessions.StartSessionAsync()).IsSuccess.Should().BeTrue();
        }

        using var check = Scope();
        var count = await check.ServiceProvider.GetRequiredService<ISessionService>().CountAsync();
        count.Value.Should().Be(2);
    }

    [Fact]
    public async Task Favorites_AddIsIdempotentAndQueryable()
    {
        using var scope = Scope();
        var favorites = scope.ServiceProvider.GetRequiredService<IFavoriteService>();

        await favorites.AddAsync(FavoriteKind.Command, "apps.open", "Abrir aplicación");
        await favorites.AddAsync(FavoriteKind.Command, "apps.open", "Abrir aplicación");

        (await favorites.IsFavoriteAsync(FavoriteKind.Command, "apps.open")).Value.Should().BeTrue();
        (await favorites.GetAsync(FavoriteKind.Command)).Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task History_RecordCommand_ReturnsRecent()
    {
        using var scope = Scope();
        var history = scope.ServiceProvider.GetRequiredService<IHistoryService>();

        await history.RecordCommandAsync("apps.open", "Abrir aplicación", "apps.open notepad", "Success", true, 12, null);
        await history.RecordCommandAsync("system.lock", "Bloquear", "system.lock", "Success", true, 3, null);

        var recent = await history.GetRecentCommandsAsync(10);
        recent.Value.Should().HaveCount(2);
        recent.Value[0].CommandId.Should().Be("system.lock");
    }

    [Fact]
    public async Task Projects_Remember_UpsertsAndIncrementsOpenCount()
    {
        using var scope = Scope();
        var projects = scope.ServiceProvider.GetRequiredService<IProjectMemoryService>();

        await projects.RememberAsync("orion", "C:\\repos\\orion");
        var second = await projects.RememberAsync("orion", "C:\\repos\\orion");

        second.Value.OpenCount.Should().Be(2);
        (await projects.CountAsync()).Value.Should().Be(1);
    }

    [Fact]
    public async Task Search_FindsMemoryItemByText()
    {
        using (var scope = Scope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IMemoryRepository<MemoryItem>>();
            await repo.AddAsync(new MemoryItem { Type = MemoryItemType.Note, Title = "Reunión importante", Content = "Revisar el despliegue", Category = "trabajo" });
            await repo.SaveChangesAsync();
        }

        using var searchScope = Scope();
        var search = searchScope.ServiceProvider.GetRequiredService<ISearchMemoryService>();
        var results = await search.SearchAsync(new MemoryQuery(Text: "Reunión"));

        results.Value.Should().Contain(r => r.Kind == "Nota" && r.Title.Contains("Reunión"));
    }
}
