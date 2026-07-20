using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Orion.Memory.Engine;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel del Memory Center: agrega conversaciones, proyectos, favoritos,
/// archivos recientes y búsqueda desde el Memory Engine (servicios con ámbito,
/// resueltos por operación).
/// </summary>
public sealed partial class MemoryCenterViewModel(IServiceScopeFactory scopeFactory) : ObservableObject
{
    public ObservableCollection<Conversation> Conversations { get; } = [];

    public ObservableCollection<Project> Projects { get; } = [];

    public ObservableCollection<Favorite> Favorites { get; } = [];

    public ObservableCollection<RecentFile> RecentFiles { get; } = [];

    public ObservableCollection<MemorySearchResult> SearchResults { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var engine = scope.ServiceProvider.GetRequiredService<IMemoryEngine>();

        var conversations = await engine.Conversations.GetRecentAsync(20).ConfigureAwait(true);
        Sync(Conversations, conversations.IsSuccess ? conversations.Value : []);

        var projects = await engine.Projects.GetRecentAsync(20).ConfigureAwait(true);
        Sync(Projects, projects.IsSuccess ? projects.Value : []);

        var favorites = await engine.Favorites.GetAsync(null).ConfigureAwait(true);
        Sync(Favorites, favorites.IsSuccess ? favorites.Value : []);

        var files = await engine.History.GetRecentFilesAsync(30).ConfigureAwait(true);
        Sync(RecentFiles, files.IsSuccess ? files.Value : []);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var engine = scope.ServiceProvider.GetRequiredService<IMemoryEngine>();

        var result = await engine.Search.SearchAsync(new MemoryQuery(Text: SearchText)).ConfigureAwait(true);
        Sync(SearchResults, result.IsSuccess ? result.Value : []);
        Status = result.IsSuccess ? $"{SearchResults.Count} resultado(s)." : "La búsqueda falló.";
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var engine = scope.ServiceProvider.GetRequiredService<IMemoryEngine>();

        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = System.IO.Path.Combine(root, "OrionAI", "exports",
            $"memory-{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}.json");

        var result = await engine.Export.ExportJsonAsync(path).ConfigureAwait(true);
        Status = result.IsSuccess ? $"Memoria exportada a {result.Value}" : $"Error: {result.Error.Message}";
    }

    private static void Sync<T>(ObservableCollection<T> target, IReadOnlyList<T> source)
    {
        target.Clear();
        foreach (var item in source)
        {
            target.Add(item);
        }
    }
}
