using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class LogsPage : Page
{
    public LogsPage()
    {
        ViewModel = App.Services.GetRequiredService<LogsViewModel>();
        InitializeComponent();
    }

    public LogsViewModel ViewModel { get; }
}
