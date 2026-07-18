using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class CommandsPage : Page
{
    public CommandsPage()
    {
        ViewModel = App.Services.GetRequiredService<CommandsViewModel>();
        InitializeComponent();
    }

    public CommandsViewModel ViewModel { get; }
}
