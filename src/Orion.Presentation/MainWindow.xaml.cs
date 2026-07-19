using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Orion.Presentation.Services;
using Orion.Presentation.ViewModels;
using Windows.System;

namespace Orion.Presentation;

/// <summary>
/// Ventana principal: barra de título personalizada (con caption buttons
/// nativos), backdrop Mica, sidebar de navegación, barra de estado y la
/// Command Palette (Ctrl+Shift+P). El code-behind solo contiene cromática de
/// ventana y enrutado de teclado (responsabilidad de la vista).
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        ViewModel = App.Services.GetRequiredService<ShellViewModel>();
        Palette = App.Services.GetRequiredService<CommandPaletteViewModel>();
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

    public CommandPaletteViewModel Palette { get; }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTitleBarInset();
        ViewModel.StartClock(DispatcherQueue);
        ViewModel.NavigateToDefault();
        App.Services.GetRequiredService<DialogService>().Initialize(DispatcherQueue, RootGrid.XamlRoot);
    }

    private async void OpenPalette_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        await Palette.OpenAsync();
        PaletteSearch.Focus(FocusState.Programmatic);
    }

    private void PaletteSearch_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Enter:
                Palette.RunTextCommand.Execute(null);
                e.Handled = true;
                break;
            case VirtualKey.Escape:
                Palette.Close();
                e.Handled = true;
                break;
        }
    }

    /// <summary>Reserva a la derecha el ancho de los botones de sistema (min/max/cerrar).</summary>
    private void UpdateTitleBarInset()
    {
        var scale = RootGrid.XamlRoot?.RasterizationScale ?? 1.0;
        RightInsetColumn.Width = new GridLength(AppWindow.TitleBar.RightInset / scale);
    }
}
