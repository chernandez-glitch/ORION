using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class MemoryCenterPage : Page
{
    public MemoryCenterPage()
    {
        ViewModel = App.Services.GetRequiredService<MemoryCenterViewModel>();
        InitializeComponent();
        Loaded += async (_, _) => await ViewModel.RefreshAsync();
    }

    public MemoryCenterViewModel ViewModel { get; }
}
