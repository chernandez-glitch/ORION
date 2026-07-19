using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Application.Commands;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel de la vista de comandos: lista los comandos disponibles y ejecuta
/// la línea escrita por el usuario a través del <see cref="ICommandExecutor"/>.
/// </summary>
public sealed partial class CommandsViewModel : ObservableObject
{
    private readonly ICommandExecutor _executor;

    public CommandsViewModel(ICommandRegistry registry, ICommandExecutor executor)
    {
        _executor = executor;
        Commands = new ObservableCollection<CommandInfo>(registry.Commands);
    }

    public ObservableCollection<CommandInfo> Commands { get; }

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
            var result = await _executor.ExecuteAsync(Input).ConfigureAwait(true);
            LastSucceeded = result.IsSuccess;
            Output = result.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
