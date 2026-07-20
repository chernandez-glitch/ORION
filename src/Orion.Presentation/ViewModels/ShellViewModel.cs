using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Orion.Configuration;
using Orion.Presentation.Models;
using Orion.Presentation.Services;
using Orion.Shared.Modules;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel del shell (ventana principal): navegación, estados de la top bar,
/// reloj, tema y datos de la barra inferior.
/// </summary>
public sealed partial class ShellViewModel : ObservableObject
{
    private const int MoonGlyph = 0xE708;
    private const int SunGlyph = 0xE706;

    private readonly INavigationService _navigation;
    private readonly IThemeService _theme;
    private DispatcherQueueTimer? _clock;

    public ShellViewModel(
        INavigationService navigation,
        IThemeService theme,
        IConfigurationService configuration,
        IEnumerable<IModuleStatusProvider> statusProviders)
    {
        _navigation = navigation;
        _theme = theme;

        MainNavItems =
        [
            new NavItem("dashboard", "Dashboard", 0xE80F),
            new NavItem("conversations", "Conversaciones", 0xE8BD),
            new NavItem("automation", "Automation Center", 0xE945),
            new NavItem("commands", "Comandos", 0xE756),
            new NavItem("memory", "Memory Center", 0xE81C),
            new NavItem("voice", "Voice Center", 0xE720),
            new NavItem("plugins", "Plugins", 0xEA86),
            new NavItem("logs", "Logs", 0xE7C3)
        ];

        FooterNavItems =
        [
            new NavItem("settings", "Configuración", 0xE713),
            new NavItem("about", "Acerca de", 0xE783)
        ];

        var byModule = statusProviders
            .Select(p => p.GetStatus())
            .ToDictionary(r => r.Module, StringComparer.OrdinalIgnoreCase);

        StatusItems =
        [
            Pill("Sistema", ModuleStatus.Ready, "Operativo"),
            ToPill(Resolve(byModule, "IA")),
            ToPill(Relabel(Resolve(byModule, "Micrófono"), "Voz")),
            ToPill(Resolve(byModule, "Memoria")),
            ToPill(Resolve(byModule, "Plugins"))
        ];

        Version = "v0.1.0";
        AiProviderText = $"IA: {configuration.Current.AI.Provider}";
        Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        _themeGlyph = char.ConvertFromUtf32(_theme.Current == ThemePreference.Dark ? SunGlyph : MoonGlyph);
    }

    public ObservableCollection<NavItem> MainNavItems { get; }

    public ObservableCollection<NavItem> FooterNavItems { get; }

    public ObservableCollection<StatusPillItem> StatusItems { get; }

    public string Version { get; }

    [ObservableProperty]
    private NavItem? _selectedNavItem;

    [ObservableProperty]
    private string _aiProviderText;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private string _statusText = "Listo";

    [ObservableProperty]
    private string _themeGlyph;

    /// <summary>Arranca el reloj de la barra inferior (en el hilo de UI).</summary>
    public void StartClock(DispatcherQueue dispatcherQueue)
    {
        _clock = dispatcherQueue.CreateTimer();
        _clock.Interval = TimeSpan.FromSeconds(1);
        _clock.Tick += (_, _) => Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        _clock.Start();
    }

    /// <summary>Selecciona la primera sección tras inicializar la navegación.</summary>
    public void NavigateToDefault() => SelectedNavItem = MainNavItems[0];

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (value is not null)
        {
            _navigation.NavigateTo(value.Key);
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _theme.Toggle();
        ThemeGlyph = char.ConvertFromUtf32(_theme.Current == ThemePreference.Dark ? SunGlyph : MoonGlyph);
    }

    [RelayCommand]
    private void OpenSettings() => SelectedNavItem = FooterNavItems[0];

    private static ModuleStatusReport Resolve(Dictionary<string, ModuleStatusReport> byModule, string module) =>
        byModule.TryGetValue(module, out var report)
            ? report
            : new ModuleStatusReport(module, ModuleStatus.Disabled, "—");

    private static ModuleStatusReport Relabel(ModuleStatusReport report, string label) =>
        report with { Module = label };

    private static StatusPillItem ToPill(ModuleStatusReport report) =>
        Pill(report.Module, report.Status, report.Detail);

    private static StatusPillItem Pill(string module, ModuleStatus status, string detail) =>
        new(module, detail, new SolidColorBrush(ColorFor(status)));

    private static global::Windows.UI.Color ColorFor(ModuleStatus status) => status switch
    {
        ModuleStatus.Ready => Colors.SeaGreen,
        ModuleStatus.Degraded => Colors.Goldenrod,
        ModuleStatus.Offline => Colors.IndianRed,
        _ => Colors.Gray
    };
}
