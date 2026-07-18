using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre el navegador, opcionalmente en una URL concreta.</summary>
public sealed class OpenBrowserCommand(IProcessAutomation process) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "abrir-navegador",
        "Abre el navegador en la URL indicada (por defecto una página en blanco).",
        CommandCategory.Web,
        "navegador", "web", "browser");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        var url = request.FirstArgument ?? "about:blank";
        var result = await process.LaunchAsync(url, arguments: null, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok($"Abriendo '{url}' en el navegador."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
