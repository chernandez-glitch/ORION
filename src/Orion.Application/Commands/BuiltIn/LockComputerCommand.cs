using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Bloquea la sesión de Windows.</summary>
public sealed class LockComputerCommand(IPowerAutomation power) : CommandBase
{
    public override string Id => "system.lock";

    public override string Name => "Bloquear equipo";

    public override string Description => "Bloquea la sesión de Windows.";

    public override CommandCategory Category => CommandCategory.System;

    public override IReadOnlyList<string> Aliases => ["bloquear", "lock"];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = await power.LockAsync(context.CancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CommandResult.Success("Sesión bloqueada.")
            : CommandResult.Failed(result.Error.Message);
    }
}
