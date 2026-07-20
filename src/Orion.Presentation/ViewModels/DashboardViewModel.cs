using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Orion.Configuration;
using Orion.Memory.Engine.Abstractions;
using Orion.Presentation.Models;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// Dashboard con widgets reales del sistema (CPU/RAM/disco/red/procesos) del
/// <see cref="ISystemMonitor"/> y tarjetas de memoria (conversaciones, sesiones,
/// proyectos, favoritos, archivos recientes) del Memory Engine.
/// </summary>
public sealed partial class DashboardViewModel(
    IConfigurationService configuration,
    ISystemMonitor monitor,
    IServiceScopeFactory scopeFactory) : ObservableObject
{
    public ObservableCollection<StatCard> Cards { get; } = [];

    public ObservableCollection<ActivityEvent> Activity { get; } =
    [
        new ActivityEvent("09:20", "Configuración guardada", "Tema cambiado a oscuro", 0xE713),
        new ActivityEvent("09:15", "Comando ejecutado", "abrir-app notepad", 0xE756),
        new ActivityEvent("09:12", "Memoria lista", "SQLite inicializado", 0xE81C),
        new ActivityEvent("09:12", "ORION iniciado", "sesión registrada", 0xE945)
    ];

    public async Task RefreshAsync()
    {
        var result = monitor.GetLoad();
        var load = result.IsSuccess ? result.Value : SystemLoad.Empty;
        var ai = configuration.Current.AI;
        var (sessions, conversations, projects, favorites, recentFiles) = await LoadMemoryCountsAsync().ConfigureAwait(true);

        var cards = new[]
        {
            new StatCard("CPU", Inv($"{load.CpuPercent:0} %"), "Uso del sistema", 0xE9D9),
            new StatCard("Memoria RAM", Inv($"{load.RamPercent:0} %"), Inv($"{load.RamUsedMb:0} de {load.RamTotalMb:0} MB"), 0xE964),
            new StatCard("Disco", Inv($"{load.DiskPercent:0} %"), Inv($"{load.DiskUsedGb:0} de {load.DiskTotalGb:0} GB"), 0xEDA2),
            new StatCard("Procesos", load.ProcessCount.ToString(CultureInfo.InvariantCulture), "activos", 0xE9D2),
            new StatCard("Conversaciones", conversations.ToString(CultureInfo.InvariantCulture), "recordadas", 0xE8BD),
            new StatCard("Sesiones", sessions.ToString(CultureInfo.InvariantCulture), "registradas", 0xE823),
            new StatCard("Proyectos", projects.ToString(CultureInfo.InvariantCulture), "en memoria", 0xE8F1),
            new StatCard("Favoritos", favorites.ToString(CultureInfo.InvariantCulture), "marcados", 0xE734),
            new StatCard("Archivos recientes", recentFiles.ToString(CultureInfo.InvariantCulture), "accedidos", 0xE8A5),
            new StatCard("Proveedor IA", ai.Provider, "configurado", 0xE99A)
        };

        Cards.Clear();
        foreach (var card in cards)
        {
            Cards.Add(card);
        }
    }

    private async Task<(int Sessions, int Conversations, int Projects, int Favorites, int RecentFiles)> LoadMemoryCountsAsync()
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var engine = scope.ServiceProvider.GetRequiredService<IMemoryEngine>();

            var sessions = await engine.Sessions.CountAsync().ConfigureAwait(true);
            var conversations = await engine.Conversations.CountAsync().ConfigureAwait(true);
            var projects = await engine.Projects.CountAsync().ConfigureAwait(true);
            var favorites = await engine.Favorites.GetAsync(null).ConfigureAwait(true);
            var files = await engine.History.GetRecentFilesAsync(1000).ConfigureAwait(true);

            return (
                sessions.IsSuccess ? sessions.Value : 0,
                conversations.IsSuccess ? conversations.Value : 0,
                projects.IsSuccess ? projects.Value : 0,
                favorites.IsSuccess ? favorites.Value.Count : 0,
                files.IsSuccess ? files.Value.Count : 0);
        }
        catch
        {
            return (0, 0, 0, 0, 0);
        }
    }

    private static string Inv(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
