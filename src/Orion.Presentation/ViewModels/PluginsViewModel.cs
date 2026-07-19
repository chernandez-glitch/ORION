using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Orion.Plugins;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// Plugins cargados (reales, vía catálogo) y las integraciones previstas
/// (simuladas) que llegan en la Fase 4.
/// </summary>
public sealed partial class PluginsViewModel : ObservableObject
{
    public PluginsViewModel(IPluginCatalog catalog)
    {
        LoadedCount = catalog.Plugins.Count;

        Planned =
        [
            new PluginItem("SAP Business One", "—", "Consultas y operaciones sobre SAP B1.", false),
            new PluginItem("SQL Server", "—", "Ejecución de consultas y scripts.", false),
            new PluginItem("Power BI", "—", "Refresco de datasets e informes.", false),
            new PluginItem("GitHub", "—", "Repos, issues y pull requests.", false),
            new PluginItem("Docker", "—", "Contenedores e imágenes.", false),
            new PluginItem("Outlook", "—", "Correo y calendario.", false),
            new PluginItem("Teams", "—", "Mensajes y reuniones.", false),
            new PluginItem("Azure", "—", "Recursos y despliegues.", false)
        ];
    }

    public ObservableCollection<PluginItem> Planned { get; }

    [ObservableProperty]
    private int _loadedCount;

    public string LoadedText => $"{LoadedCount} plugin(s) cargado(s). Las integraciones llegan en la Fase 4.";
}
