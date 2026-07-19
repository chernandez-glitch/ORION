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
    }

    public AutomationViewModel ViewModel { get; }
}
