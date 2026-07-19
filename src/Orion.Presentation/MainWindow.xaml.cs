using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Orion.Presentation.Services;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation;

/// <summary>
/// Ventana principal: barra de título personalizada (con caption buttons
/// nativos), backdrop Mica, sidebar de navegación y barra de estado. El
/// code-behind solo contiene cromática de ventana (responsabilidad de la vista);
/// la navegación, el tema y los estados viven en el ShellViewModel y los servicios.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        ViewModel = App.Services.GetRequiredService<ShellViewModel>();
        InitializeComponent();

        Title = "ORION AI";
        SystemBackdrop = new MicaBackdrop();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        var navigation = App.Services.GetRequiredService<INavigationService>();
        navigation.Initialize(ContentFrame);
        App.Services.GetRequiredService<IThemeService>().Initialize(RootGrid);

        RootGrid.Loaded += OnRootLoaded;
        AppTitleBar.SizeChanged += (_, _) => UpdateTitleBarInset();
    }

    public ShellViewModel ViewModel { get; }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTitleBarInset();
        ViewModel.StartClock(DispatcherQueue);
        ViewModel.NavigateToDefault();
    }

    /// <summary>Reserva a la derecha el ancho de los botones de sistema (min/max/cerrar).</summary>
    private void UpdateTitleBarInset()
    {
        var scale = RootGrid.XamlRoot?.RasterizationScale ?? 1.0;
        RightInsetColumn.Width = new GridLength(AppWindow.TitleBar.RightInset / scale);
    }
}
