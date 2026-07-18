using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Reinicia el equipo tras un breve margen de seguridad.</summary>
public sealed class RestartComputerCommand(IPowerAutomation power) : ICommand
{
    private static readonly TimeSpan SafetyDelay = TimeSpan.FromSeconds(15);

    public CommandDescriptor Descriptor { get; } = new(
        "reiniciar",
        "Reinicia el equipo (con 15s de margen para cancelar).",
        CommandCategory.Power,
        "restart");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        var result = await power.RestartAsync(SafetyDelay, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok("El equipo se reiniciará en 15 segundos."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
