using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        ViewModel = App.Services.GetRequiredService<AboutViewModel>();
        InitializeComponent();
    }

    public AboutViewModel ViewModel { get; }
}
