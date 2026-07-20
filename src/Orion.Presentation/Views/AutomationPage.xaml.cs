using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class AutomationPage : Page
{
    public AutomationPage()
    {
        ViewModel = App.Services.GetRequiredService<AutomationViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.RefreshAsync();
    }

    public AutomationViewModel ViewModel { get; }
}
