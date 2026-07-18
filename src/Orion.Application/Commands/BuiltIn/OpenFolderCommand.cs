using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una carpeta en el Explorador de Windows.</summary>
public sealed class OpenFolderCommand(IProcessAutomation process) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "abrir-carpeta",
        "Abre una carpeta en el Explorador (p. ej. abrir-carpeta \"C:\\Proyectos\").",
        CommandCategory.Files,
        "carpeta", "folder");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FirstArgument is not { } path)
        {
            return Result.Failure<CommandOutcome>(CommandErrors.MissingArgument("ruta"));
        }

        var result = await process.LaunchAsync("explorer.exe", path, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok($"Abriendo la carpeta '{path}'."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
