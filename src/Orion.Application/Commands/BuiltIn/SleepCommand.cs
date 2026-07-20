using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Suspende el equipo (modo reposo).</summary>
public sealed class SleepCommand(ISystemService system) : CommandBase
{
    public override string Id => "system.sleep";

    public override string Name => "Suspender equipo";

    public override string Description => "Pone el equipo en modo reposo (suspender).";

    public override CommandCategory Category => CommandCategory.System;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["suspender", "sleep", "reposo"];

    public override Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = system.Suspend();
        return Task.FromResult(result.IsSuccess
            ? CommandResult.Success("Suspendiendo el equipo…")
            : CommandResult.Failed(result.Error.Message));
    }
}
