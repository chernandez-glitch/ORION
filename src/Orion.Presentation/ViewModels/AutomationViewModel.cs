using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Orion.Application.Commands;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// Automation Center: procesos, ventanas, acciones recientes y estado del
/// sistema en tiempo real, sobre el Windows Automation Engine.
/// </summary>
public sealed partial class AutomationViewModel(
    IProcessService processes,
    IWindowService windows,
    ISystemService system,
    ISystemMonitor monitor,
    IServiceScopeFactory scopeFactory) : ObservableObject
{
    public ObservableCollection<ProcessDetails> Processes { get; } = [];

    public ObservableCollection<WindowDetails> Windows { get; } = [];

    public ObservableCollection<string> RecentActions { get; } = [];

    [ObservableProperty]
    private string _systemStatus = "Cargando…";

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var all = await processes.GetAllAsync().ConfigureAwait(true);
        Processes.Clear();
        if (all.IsSuccess)
        {
            foreach (var process in all.Value.Take(15))
            {
                Processes.Add(process);
            }
        }

        var openWindows = windows.GetOpenWindows();
        Windows.Clear();
        if (openWindows.IsSuccess)
        {
            foreach (var window in openWindows.Value.Take(15))
            {
                Windows.Add(window);
            }
        }

        var info = system.GetSystemInfo();
        var load = monitor.GetLoad();
        if (info.IsSuccess && load.IsSuccess)
        {
            SystemStatus = string.Create(CultureInfo.InvariantCulture,
                $"{info.Value.UserName}@{info.Value.MachineName} · {info.Value.OsDescription} · CPU {load.Value.CpuPercent:0}% · RAM {load.Value.RamPercent:0}% · {load.Value.ProcessCount} procesos");
        }

        await LoadRecentActionsAsync().ConfigureAwait(true);
    }

    private async Task LoadRecentActionsAsync()
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var history = scope.ServiceProvider.GetRequiredService<ICommandHistory>();
            var recent = await history.GetRecentAsync(10).ConfigureAwait(true);

            RecentActions.Clear();
            foreach (var entry in recent)
            {
                var mark = entry.Status == CommandStatus.Success ? "✓" : "✕";
                RecentActions.Add($"{mark}  {entry.CommandName} — {entry.Message}");
            }
        }
        catch
        {
            RecentActions.Clear();
        }
    }
}
