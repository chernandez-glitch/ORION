using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una carpeta en el Explorador de Windows.</summary>
public sealed class OpenFolderCommand(IProcessAutomation process) : CommandBase
{
    public override string Id => "folders.open";

    public override string Name => "Abrir carpeta";

    public override string Description => "Abre una carpeta en el Explorador de Windows.";

    public override CommandCategory Category => CommandCategory.Folders;

    public override IReadOnlyList<string> Aliases => ["abrir-carpeta", "carpeta", "open folder", "folder"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("path", "Ruta de la carpeta.", IsRequired: true, Example: @"C:\Proyectos")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var path = context.GetParameter("path")!;
        var result = await process.LaunchAsync("explorer.exe", path, context.CancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? CommandResult.Success($"Abriendo la carpeta '{path}'.")
            : CommandResult.Failed(result.Error.Message);
    }
}
