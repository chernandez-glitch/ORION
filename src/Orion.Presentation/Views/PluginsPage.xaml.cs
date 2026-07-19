using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class PluginsPage : Page
{
    public PluginsPage()
    {
        ViewModel = App.Services.GetRequiredService<PluginsViewModel>();
        InitializeComponent();
    }

    public PluginsViewModel ViewModel { get; }
}
