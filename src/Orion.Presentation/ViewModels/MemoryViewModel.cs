using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Application.Memory;
using Orion.Application.Memory.Dtos;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel de la memoria: muestra y edita proyectos favoritos, rutas y el
/// historial de comandos que ORION recuerda.
/// </summary>
public sealed partial class MemoryViewModel(IMemoryService memory) : ObservableObject
{
    public ObservableCollection<FavoriteProjectDto> Projects { get; } = [];

    public ObservableCollection<FavoriteRouteDto> Routes { get; } = [];

    public ObservableCollection<CommandHistoryDto> History { get; } = [];

    [ObservableProperty]
    private string _newProjectName = string.Empty;

    [ObservableProperty]
    private string _newProjectPath = string.Empty;

    [ObservableProperty]
    private string _newRouteAlias = string.Empty;

    [ObservableProperty]
    private string _newRoutePath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var projects = await memory.GetProjectsAsync().ConfigureAwait(true);
        if (projects.IsSuccess)
        {
            Sync(Projects, projects.Value);
        }

        var routes = await memory.GetRoutesAsync().ConfigureAwait(true);
        if (routes.IsSuccess)
        {
            Sync(Routes, routes.Value);
        }

        var history = await memory.GetRecentCommandsAsync(20).ConfigureAwait(true);
        if (history.IsSuccess)
        {
            Sync(History, history.Value);
        }
    }

    [RelayCommand]
    private async Task AddProjectAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProjectName) || string.IsNullOrWhiteSpace(NewProjectPath))
        {
            StatusMessage = "Indica nombre y ruta del proyecto.";
            return;
        }

        var result = await memory.AddProjectAsync(NewProjectName, NewProjectPath).ConfigureAwait(true);
        if (result.IsSuccess)
        {
            Projects.Add(result.Value);
            NewProjectName = string.Empty;
            NewProjectPath = string.Empty;
            StatusMessage = "Proyecto recordado.";
        }
        else
        {
            StatusMessage = result.Error.Message;
        }
    }

    [RelayCommand]
    private async Task AddRouteAsync()
    {
        if (string.IsNullOrWhiteSpace(NewRouteAlias) || string.IsNullOrWhiteSpace(NewRoutePath))
        {
            StatusMessage = "Indica alias y ruta.";
            return;
        }

        var result = await memory.AddRouteAsync(NewRouteAlias, NewRoutePath).ConfigureAwait(true);
        if (result.IsSuccess)
        {
            Routes.Add(result.Value);
            NewRouteAlias = string.Empty;
            NewRoutePath = string.Empty;
            StatusMessage = "Ruta recordada.";
        }
        else
        {
            StatusMessage = result.Error.Message;
        }
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
