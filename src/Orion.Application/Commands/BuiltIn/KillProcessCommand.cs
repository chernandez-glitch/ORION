using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Termina forzosamente un proceso (con guarda de seguridad).</summary>
public sealed class KillProcessCommand(IProcessService process) : CommandBase
{
    public override string Id => "process.kill";

    public override string Name => "Terminar proceso";

    public override string Description => "Termina forzosamente un proceso por su nombre (protege procesos críticos).";

    public override CommandCategory Category => CommandCategory.System;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["matar-proceso", "kill", "terminar"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("process", "Nombre del proceso.", IsRequired: true, Example: "notepad")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var name = context.GetParameter("process")!;
        var result = await process.KillAsync(name, context.CancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CommandResult.Warning($"Proceso '{name}' terminado.")
            : CommandResult.Failed(result.Error.Message);
    }
}
