using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>Automatizaciones guardadas. Datos simulados; la ejecución llega en Fase 1.</summary>
public sealed partial class AutomationViewModel : ObservableObject
{
    public AutomationViewModel()
    {
        Automations =
        [
            new AutomationItem("Backup nocturno", "Cada día a las 22:00", true),
            new AutomationItem("Abrir entorno de trabajo", "Al decir \"buenos días\"", true),
            new AutomationItem("Cerrar apps de distracción", "Al iniciar modo enfoque", false)
        ];
    }

    public ObservableCollection<AutomationItem> Automations { get; }

    [ObservableProperty]
    private string _hint = "La ejecución real de automatizaciones se habilita en la Fase 1.";
}
