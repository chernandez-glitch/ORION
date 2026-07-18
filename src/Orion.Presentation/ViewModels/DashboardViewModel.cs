using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Orion.Application.Dashboard;
using Orion.Application.Memory.Dtos;
using Orion.Shared.Modules;

namespace Orion.Presentation.ViewModels;

/// <summary>ViewModel del dashboard: estado de módulos, CPU/RAM y últimos comandos.</summary>
public sealed partial class DashboardViewModel(IDashboardService dashboard, ILogger<DashboardViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    private double _cpuPercent;

    [ObservableProperty]
    private double _ramUsedMb;

    [ObservableProperty]
    private double _ramTotalMb;

    [ObservableProperty]
    private double _ramPercent;

    [ObservableProperty]
    private int _registeredCommandCount;

    [ObservableProperty]
    private string _cpuText = "0 %";

    [ObservableProperty]
    private string _ramText = "0 %";

    [ObservableProperty]
    private string _ramDetailText = "—";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasNoHistory = true;

    public ObservableCollection<ModuleStatusReport> Modules { get; } = [];

    public ObservableCollection<CommandHistoryDto> RecentCommands { get; } = [];

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsLoading = true;
        try
        {
            var result = await dashboard.GetSnapshotAsync().ConfigureAwait(true);
            if (result.IsFailure)
            {
                logger.LogWarning("No se pudo cargar el dashboard: {Error}", result.Error);
                return;
            }

            var snapshot = result.Value;
            CpuPercent = snapshot.Metrics.CpuPercent;
            RamUsedMb = snapshot.Metrics.RamUsedMb;
            RamTotalMb = snapshot.Metrics.RamTotalMb;
            RamPercent = snapshot.Metrics.RamPercent;
            RegisteredCommandCount = snapshot.RegisteredCommandCount;

            CpuText = string.Create(CultureInfo.InvariantCulture, $"{snapshot.Metrics.CpuPercent:0.0} %");
            RamText = string.Create(CultureInfo.InvariantCulture, $"{snapshot.Metrics.RamPercent:0.0} %");
            RamDetailText = string.Create(CultureInfo.InvariantCulture, $"{snapshot.Metrics.RamUsedMb:0} MB en uso");

            Sync(Modules, snapshot.Modules);
            Sync(RecentCommands, snapshot.RecentCommands);
            HasNoHistory = RecentCommands.Count == 0;
        }
        finally
        {
            IsLoading = false;
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
