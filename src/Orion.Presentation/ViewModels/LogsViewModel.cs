using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>Visor de logs. Datos simulados que representan la actividad del sistema.</summary>
public sealed partial class LogsViewModel : ObservableObject
{
    public LogsViewModel() => Reload();

    public ObservableCollection<LogEntry> Logs { get; } = [];

    [RelayCommand]
    private void Reload()
    {
        Logs.Clear();
        foreach (var entry in Sample())
        {
            Logs.Add(entry);
        }
    }

    private static IEnumerable<LogEntry> Sample() =>
    [
        new LogEntry("09:12:03", "INFO", "ORION AI iniciado correctamente."),
        new LogEntry("09:12:03", "INFO", "Motor de comandos: 8 comandos registrados."),
        new LogEntry("09:12:04", "INFO", "Sistema de memoria listo (SQLite)."),
        new LogEntry("09:14:22", "INFO", "Comando 'abrir-app' ejecutado."),
        new LogEntry("09:15:01", "WARN", "Automatización 'process.launch' no implementada en esta fase."),
        new LogEntry("09:20:47", "INFO", "Configuración guardada.")
    ];
}
