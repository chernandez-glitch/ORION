using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.Views;

namespace Orion.Presentation;

/// <summary>
/// Ventana principal: alberga la barra lateral de navegación (NavigationView) y
/// un Frame donde se muestran las páginas. La navegación mapea el Tag del ítem
/// al tipo de página; cada página resuelve su ViewModel desde el contenedor.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = "ORION AI";
        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        var tag = (args.InvokedItemContainer?.Tag as string) ?? "dashboard";
        var pageType = tag switch
        {
            "commands" => typeof(CommandsPage),
            "memory" => typeof(MemoryPage),
            _ => typeof(DashboardPage)
        };

        if (ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType);
        }
    }
}
