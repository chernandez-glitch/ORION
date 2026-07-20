using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Orion.Configuration;
using Orion.Presentation.Models;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// Dashboard con widgets reales del sistema (CPU, RAM, disco, red, procesos)
/// tomados de <see cref="ISystemMonitor"/>, más el estado de IA/plugins.
/// </summary>
public sealed partial class DashboardViewModel(IConfigurationService configuration, ISystemMonitor monitor) : ObservableObject
{
    public ObservableCollection<StatCard> Cards { get; } = [];

    public ObservableCollection<ActivityEvent> Activity { get; } =
    [
        new ActivityEvent("09:20", "Configuración guardada", "Tema cambiado a oscuro", 0xE713),
        new ActivityEvent("09:15", "Comando ejecutado", "abrir-app notepad", 0xE756),
        new ActivityEvent("09:12", "Motor de comandos", "comandos registrados", 0xE945),
        new ActivityEvent("09:12", "ORION iniciado", "SQLite listo", 0xE81C)
    ];

    public void Refresh()
    {
        var result = monitor.GetLoad();
        var load = result.IsSuccess ? result.Value : SystemLoad.Empty;
        var ai = configuration.Current.AI;

        var cards = new[]
        {
            new StatCard("CPU", Inv($"{load.CpuPercent:0} %"), "Uso del sistema", 0xE9D9),
            new StatCard("Memoria RAM", Inv($"{load.RamPercent:0} %"), Inv($"{load.RamUsedMb:0} de {load.RamTotalMb:0} MB"), 0xE964),
            new StatCard("Disco", Inv($"{load.DiskPercent:0} %"), Inv($"{load.DiskUsedGb:0} de {load.DiskTotalGb:0} GB"), 0xEDA2),
            new StatCard("Red", Inv($"{load.NetworkKbps:0} KB/s"), "tráfico actual", 0xE968),
            new StatCard("Procesos", load.ProcessCount.ToString(CultureInfo.InvariantCulture), "activos", 0xE9D2),
            new StatCard("Proveedor IA", ai.Provider, "configurado", 0xE99A),
            new StatCard("Modelo IA", ai.Model, "Fase 2", 0xE8D7),
            new StatCard("Plugins", "0", "cargados", 0xEA86)
        };

        Cards.Clear();
        foreach (var card in cards)
        {
            Cards.Add(card);
        }
    }

    private static string Inv(FormattableString text) => text.ToString(CultureInfo.InvariantCulture);
}
