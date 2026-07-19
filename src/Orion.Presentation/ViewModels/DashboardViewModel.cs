using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Orion.Configuration;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// Dashboard con métricas de un vistazo. En esta entrega los valores son
/// simulados (no se conectan servicios); el proveedor y modelo de IA se leen de
/// la configuración para reflejar los ajustes reales.
/// </summary>
public sealed partial class DashboardViewModel : ObservableObject
{
    public DashboardViewModel(IConfigurationService configuration)
    {
        var ai = configuration.Current.AI;

        Cards =
        [
            new StatCard("CPU", "23 %", "Uso del sistema", 0xE9D9),
            new StatCard("Memoria RAM", "6.2 GB", "de 16 GB", 0xE964),
            new StatCard("Micrófono", "Desactivado", "Voz en Fase 3", 0xE720),
            new StatCard("Proveedor IA", ai.Provider, "Configurado", 0xE99A),
            new StatCard("Modelo IA", ai.Model, "Listo en Fase 2", 0xE8D7),
            new StatCard("Plugins", "0", "cargados", 0xEA86),
            new StatCard("Comandos", "128", "ejecutados", 0xE756),
            new StatCard("Automatizaciones", "3", "definidas", 0xE945),
            new StatCard("Última actividad", "hace 2 min", "abrir-app notepad", 0xE823),
            new StatCard("Tiempo activo", "1h 24m", "esta sesión", 0xE916)
        ];

        Activity =
        [
            new ActivityEvent("09:20", "Configuración guardada", "Tema cambiado a oscuro", 0xE713),
            new ActivityEvent("09:15", "Comando ejecutado", "abrir-app notepad", 0xE756),
            new ActivityEvent("09:14", "Automatización", "process.launch no disponible (Fase 1)", 0xE945),
            new ActivityEvent("09:12", "Memoria lista", "SQLite inicializado", 0xE81C),
            new ActivityEvent("09:12", "ORION iniciado", "8 comandos registrados", 0xE945)
        ];
    }

    public ObservableCollection<StatCard> Cards { get; }

    public ObservableCollection<ActivityEvent> Activity { get; }
}
