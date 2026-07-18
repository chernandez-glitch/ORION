using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class MemoryPage : Page
{
    public MemoryPage()
    {
        ViewModel = App.Services.GetRequiredService<MemoryViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.RefreshAsync();
    }

    public MemoryViewModel ViewModel { get; }
}
