using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class DashboardPage : Page
{
    private readonly DispatcherQueueTimer _timer;

    public DashboardPage()
    {
        ViewModel = App.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();

        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(2);
        _timer.Tick += (_, _) => ViewModel.Refresh();

        Loaded += (_, _) => { ViewModel.Refresh(); _timer.Start(); };
        Unloaded += (_, _) => _timer.Stop();
    }

    public DashboardViewModel ViewModel { get; }
}
