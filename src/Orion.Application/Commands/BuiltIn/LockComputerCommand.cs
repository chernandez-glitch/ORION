using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Bloquea la sesión de Windows.</summary>
public sealed class LockComputerCommand(IPowerAutomation power) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "bloquear",
        "Bloquea la sesión de Windows.",
        CommandCategory.Power,
        "lock");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        var result = await power.LockAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok("Sesión bloqueada."))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
