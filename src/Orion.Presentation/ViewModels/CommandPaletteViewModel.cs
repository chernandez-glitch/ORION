using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Orion.Application.Commands;
using Orion.Configuration;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel de la Command Palette (Ctrl+Shift+P): busca comandos por nombre,
/// categoría, descripción y alias; permite favoritos y muestra recientes; y
/// ejecuta a través del <see cref="ICommandExecutor"/> (nunca directamente).
/// </summary>
public sealed partial class CommandPaletteViewModel : ObservableObject
{
    private readonly ICommandRegistry _registry;
    private readonly ICommandExecutor _executor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfigurationService _configuration;
    private readonly HashSet<string> _favorites;

    public CommandPaletteViewModel(
        ICommandRegistry registry,
        ICommandExecutor executor,
        IServiceScopeFactory scopeFactory,
        IConfigurationService configuration)
    {
        _registry = registry;
        _executor = executor;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _favorites = new HashSet<string>(configuration.Current.FavoriteCommandIds, StringComparer.OrdinalIgnoreCase);
    }

    public ObservableCollection<PaletteItem> Results { get; } = [];

    public ObservableCollection<string> Recents { get; } = [];

    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _statusIsError;

    public bool ShowRecents => Recents.Count > 0 && string.IsNullOrWhiteSpace(SearchText);

    // Propiedades de visibilidad tipadas (x:Bind en una Window no admite convertidores).
    public Visibility OverlayVisibility => IsOpen ? Visibility.Visible : Visibility.Collapsed;

    public Visibility RecentsVisibility => ShowRecents ? Visibility.Visible : Visibility.Collapsed;

    public async Task OpenAsync()
    {
        SearchText = string.Empty;
        StatusMessage = string.Empty;
        UpdateResults();
        await LoadRecentsAsync().ConfigureAwait(true);
        IsOpen = true;
    }

    public void Close() => IsOpen = false;

    partial void OnIsOpenChanged(bool value) => OnPropertyChanged(nameof(OverlayVisibility));

    partial void OnSearchTextChanged(string value)
    {
        UpdateResults();
        OnPropertyChanged(nameof(ShowRecents));
        OnPropertyChanged(nameof(RecentsVisibility));
    }

    [RelayCommand]
    private async Task RunTextAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return;
        }

        var result = await _executor.ExecuteAsync(SearchText).ConfigureAwait(true);
        ApplyResult(result);
    }

    [RelayCommand]
    private async Task InvokeAsync(PaletteItem? item)
    {
        if (item is null)
        {
            return;
        }

        if (item.RequiresParameters)
        {
            // Deja al usuario completar los parámetros en la caja de búsqueda.
            SearchText = $"{item.Info.PrimaryKey} ";
            StatusMessage = $"Añade: {string.Join(", ", item.Info.Parameters.Select(p => p.Name))}";
            StatusIsError = false;
            return;
        }

        var result = await _executor
            .ExecuteAsync(item.Info.Id, new Dictionary<string, string?>())
            .ConfigureAwait(true);
        ApplyResult(result);
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(PaletteItem? item)
    {
        if (item is null)
        {
            return;
        }

        if (!_favorites.Add(item.Info.Id))
        {
            _favorites.Remove(item.Info.Id);
        }

        item.IsFavorite = _favorites.Contains(item.Info.Id);

        var settings = _configuration.Current;
        settings.FavoriteCommandIds = [.. _favorites];
        await _configuration.SaveAsync(settings).ConfigureAwait(true);
    }

    [RelayCommand]
    private void UseRecent(string? commandName)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            return;
        }

        var match = _registry.Commands.FirstOrDefault(c => c.Name == commandName);
        SearchText = match is not null ? match.PrimaryKey : commandName;
    }

    private void ApplyResult(CommandResult result)
    {
        StatusMessage = result.Message;
        StatusIsError = result.Status is CommandStatus.Failed;
        if (result.IsSuccess)
        {
            IsOpen = false;
        }
    }

    private void UpdateResults()
    {
        var firstToken = SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

        IEnumerable<CommandInfo> source = string.IsNullOrWhiteSpace(firstToken)
            ? _registry.Commands
                .OrderByDescending(c => _favorites.Contains(c.Id))
                .ThenBy(c => c.Category)
            : _registry.Search(firstToken);

        Results.Clear();
        foreach (var info in source)
        {
            Results.Add(new PaletteItem(info, _favorites.Contains(info.Id)));
        }
    }

    private async Task LoadRecentsAsync()
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var history = scope.ServiceProvider.GetRequiredService<ICommandHistory>();
            var recent = await history.GetRecentAsync(6).ConfigureAwait(true);

            Recents.Clear();
            foreach (var name in recent.Select(r => r.CommandName).Distinct().Take(6))
            {
                Recents.Add(name);
            }
        }
        catch
        {
            Recents.Clear();
        }

        OnPropertyChanged(nameof(ShowRecents));
        OnPropertyChanged(nameof(RecentsVisibility));
    }
}
