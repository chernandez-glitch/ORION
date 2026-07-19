using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Reinicia el equipo tras un breve margen de seguridad.</summary>
public sealed class RestartComputerCommand(IPowerAutomation power) : CommandBase
{
    private static readonly TimeSpan SafetyDelay = TimeSpan.FromSeconds(15);

    public override string Id => "system.restart";

    public override string Name => "Reiniciar equipo";

    public override string Description => "Reinicia el equipo (con 15s de margen para cancelar).";

    public override CommandCategory Category => CommandCategory.System;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["reiniciar", "restart"];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = await power.RestartAsync(SafetyDelay, context.CancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? CommandResult.Warning("El equipo se reiniciará en 15 segundos.")
            : CommandResult.Failed(result.Error.Message);
    }
}
