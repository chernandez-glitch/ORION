using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Apaga el equipo tras un breve margen de seguridad.</summary>
public sealed class ShutdownComputerCommand(IPowerAutomation power) : ICommand
{
    private static readonly TimeSpan SafetyDelay = TimeSpan.FromSeconds(15);

    public CommandDescriptor Descriptor { get; } = new(
        "apagar",
        "Apaga el equipo (con 15s de margen para cancelar).",
        CommandCategory.Power,
        "shutdown");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        var result = await power.ShutdownAsync(SafetyDelay, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok("El equipo se apagará en 15 segundos."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
