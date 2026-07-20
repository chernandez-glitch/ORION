using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class VoiceCenterPage : Page
{
    private readonly DispatcherQueueTimer _timer;

    public VoiceCenterPage()
    {
        ViewModel = App.Services.GetRequiredService<VoiceCenterViewModel>();
        InitializeComponent();

        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(200);
        _timer.Tick += (_, _) => ViewModel.Poll();

        Loaded += (_, _) => _timer.Start();
        Unloaded += (_, _) => _timer.Stop();
    }

    public VoiceCenterViewModel ViewModel { get; }
}
