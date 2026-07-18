using Orion.Application.Memory;
using Orion.Automation.Abstractions;
using Orion.Shared.Results;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>
/// Abre un proyecto favorito recordado en memoria (resuelve su ruta por nombre
/// y lo abre en VS Code). Ilustra la colaboración memoria + automatización.
/// </summary>
public sealed class OpenProjectCommand(IMemoryService memory, IProcessAutomation process) : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "abrir-proyecto",
        "Abre un proyecto favorito por su nombre recordado.",
        CommandCategory.Development,
        "proyecto", "project");

    public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FirstArgument is not { } name)
        {
            return Result.Failure<CommandOutcome>(CommandErrors.MissingArgument("nombre del proyecto"));
        }

        var project = await memory.GetProjectByNameAsync(name, cancellationToken).ConfigureAwait(false);
        if (project.IsFailure)
        {
            return Result.Failure<CommandOutcome>(project.Error);
        }

        var launch = await process.LaunchAsync("code", project.Value.Path, cancellationToken).ConfigureAwait(false);

        return launch.IsSuccess
            ? Result.Success(CommandOutcome.Ok($"Abriendo el proyecto '{project.Value.Name}'."))
            : Result.Failure<CommandOutcome>(launch.Error);
    }
}
