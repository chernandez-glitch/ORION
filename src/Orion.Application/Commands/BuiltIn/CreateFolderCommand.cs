using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Crea una carpeta.</summary>
public sealed class CreateFolderCommand(IFileSystemService fileSystem) : CommandBase
{
    public override string Id => "files.create-folder";

    public override string Name => "Crear carpeta";

    public override string Description => "Crea una carpeta en la ruta indicada.";

    public override CommandCategory Category => CommandCategory.Folders;

    public override IReadOnlyList<string> Aliases => ["crear-carpeta", "mkdir", "nueva-carpeta"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("path", "Ruta de la carpeta.", IsRequired: true, Example: @"C:\Temp\Nueva")];

    public override Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var path = context.GetParameter("path")!;
        var result = fileSystem.CreateFolder(path);
        return Task.FromResult(result.IsSuccess
            ? CommandResult.Success($"Carpeta creada: '{path}'.")
            : CommandResult.Failed(result.Error.Message));
    }
}
