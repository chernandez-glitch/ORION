using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Orion.Application.Commands.Abstractions;

namespace Orion.Presentation.Services;

/// <summary>
/// Implementación WinUI de <see cref="IDialogService"/>. Muestra un ContentDialog
/// moderno, marshalando siempre al hilo de UI (los comandos pueden ejecutarse
/// fuera de él).
/// </summary>
public sealed class DialogService : IDialogService
{
    private DispatcherQueue? _dispatcher;
    private XamlRoot? _xamlRoot;

    public void Initialize(DispatcherQueue dispatcher, XamlRoot xamlRoot)
    {
        _dispatcher = dispatcher;
        _xamlRoot = xamlRoot;
    }

    public Task ShowMessageAsync(string title, string message)
    {
        if (_dispatcher is null || _xamlRoot is null)
        {
            return Task.CompletedTask;
        }

        var completion = new TaskCompletionSource();

        _dispatcher.TryEnqueue(async () =>
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    CloseButtonText = "Aceptar",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = _xamlRoot
                };

                await dialog.ShowAsync();
            }
            finally
            {
                completion.TrySetResult();
            }
        });

        return completion.Task;
    }
}
