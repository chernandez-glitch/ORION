using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Application.Commands;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel de la consola de comandos: lista los comandos disponibles y ejecuta
/// la línea escrita por el usuario a través del motor de comandos.
/// </summary>
public sealed partial class CommandsViewModel : ObservableObject
{
    private readonly ICommandDispatcher _dispatcher;

    public CommandsViewModel(ICommandRegistry registry, ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        Commands = new ObservableCollection<CommandDescriptor>(
            registry.Descriptors.OrderBy(d => d.Category).ThenBy(d => d.Name));
    }

    public ObservableCollection<CommandDescriptor> Commands { get; }

    [ObservableProperty]
    private string _input = string.Empty;

    [ObservableProperty]
    private string _output = string.Empty;

    [ObservableProperty]
    private bool _lastSucceeded = true;

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task RunAsync()
    {
        if (string.IsNullOrWhiteSpace(Input))
        {
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _dispatcher.DispatchAsync(Input).ConfigureAwait(true);
            LastSucceeded = result.IsSuccess;
            Output = result.IsSuccess ? result.Value.Message : result.Error.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
