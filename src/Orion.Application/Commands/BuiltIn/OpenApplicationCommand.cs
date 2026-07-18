using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una aplicación por ruta o nombre de ejecutable.</summary>
public sealed class OpenApplicationCommand(IProcessAutomation process) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "abrir-app",
        "Abre una aplicación por ruta o nombre (p. ej. abrir-app notepad).",
        CommandCategory.System,
        "app", "open-app", "ejecutar");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FirstArgument is not { } target)
        {
            return Result.Failure<CommandOutcome>(CommandErrors.MissingArgument("aplicación"));
        }

        var arguments = request.Arguments.Count > 1 ? string.Join(' ', request.Arguments.Skip(1)) : null;
        var result = await process.LaunchAsync(target, arguments, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success(CommandOutcome.Ok($"Abriendo '{target}'.", result.Value))
            : Result.Failure<CommandOutcome>(result.Error);
    }
}
