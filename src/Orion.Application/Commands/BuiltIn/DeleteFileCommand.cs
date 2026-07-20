using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Elimina un archivo (con guarda de seguridad sobre rutas del sistema).</summary>
public sealed class DeleteFileCommand(IFileSystemService fileSystem) : CommandBase
{
    public override string Id => "files.delete-file";

    public override string Name => "Eliminar archivo";

    public override string Description => "Elimina un archivo (bloquea rutas protegidas del sistema).";

    public override CommandCategory Category => CommandCategory.Files;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["eliminar-archivo", "borrar-archivo", "del-file"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("path", "Ruta del archivo.", IsRequired: true, Example: @"C:\Temp\viejo.txt")];

    public override Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var path = context.GetParameter("path")!;
        var result = fileSystem.DeleteFile(path);
        return Task.FromResult(result.IsSuccess
            ? CommandResult.Warning($"Archivo eliminado: '{path}'.")
            : CommandResult.Failed(result.Error.Message));
    }
}
