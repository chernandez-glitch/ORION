using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Cierra de forma ordenada una aplicación por nombre de proceso.</summary>
public sealed class CloseApplicationCommand(IProcessService process) : CommandBase
{
    public override string Id => "process.close";

    public override string Name => "Cerrar aplicación";

    public override string Description => "Cierra de forma ordenada una aplicación por su nombre de proceso.";

    public override CommandCategory Category => CommandCategory.Applications;

    public override IReadOnlyList<string> Aliases => ["cerrar-app", "close-app", "cerrar"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("app", "Nombre del proceso.", IsRequired: true, Example: "notepad")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var app = context.GetParameter("app")!;
        var result = await process.CloseAsync(app, context.CancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CommandResult.Success($"Cerrando '{app}'.")
            : CommandResult.Failed(result.Error.Message);
    }
}
