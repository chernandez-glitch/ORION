using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Apaga el equipo tras un breve margen de seguridad.</summary>
public sealed class ShutdownComputerCommand(IPowerAutomation power) : CommandBase
{
    private static readonly TimeSpan SafetyDelay = TimeSpan.FromSeconds(15);

    public override string Id => "system.shutdown";

    public override string Name => "Apagar equipo";

    public override string Description => "Apaga el equipo (con 15s de margen para cancelar).";

    public override CommandCategory Category => CommandCategory.System;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["apagar", "shutdown"];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = await power.ShutdownAsync(SafetyDelay, context.CancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CommandResult.Warning("El equipo se apagará en 15 segundos.")
            : CommandResult.Failed(result.Error.Message);
    }
}
