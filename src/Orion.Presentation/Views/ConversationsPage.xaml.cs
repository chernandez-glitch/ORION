using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Orion.Presentation.ViewModels;

namespace Orion.Presentation.Views;

public sealed partial class ConversationsPage : Page
{
    public ConversationsPage()
    {
        ViewModel = App.Services.GetRequiredService<ConversationsViewModel>();
        InitializeComponent();
    }

    public ConversationsViewModel ViewModel { get; }
}
