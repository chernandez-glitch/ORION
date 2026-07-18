using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre Visual Studio Code, opcionalmente en una carpeta.</summary>
public sealed class OpenVSCodeCommand(IProcessAutomation process) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "abrir-vscode",
        "Abre Visual Studio Code en la carpeta indicada (o en la actual).",
        CommandCategory.Development,
        "vscode", "code");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        var path = request.FirstArgument ?? ".";
        var result = await process.LaunchAsync("code", path, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok($"Abriendo VS Code en '{path}'."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
