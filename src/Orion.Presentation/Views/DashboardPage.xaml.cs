using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class DashboardPage : Page
{
    private readonly DispatcherQueueTimer _refreshTimer;

    public DashboardPage()
    {
        ViewModel = App.Services.GetRequiredService<DashboardViewModel>();
        InitializeComponent();

        _refreshTimer = DispatcherQueue.CreateTimer();
        _refreshTimer.Interval = TimeSpan.FromSeconds(3);
        _refreshTimer.Tick += async (_, _) => await ViewModel.RefreshAsync();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public DashboardViewModel ViewModel { get; }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.RefreshAsync();
        _refreshTimer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => _refreshTimer.Stop();
}
