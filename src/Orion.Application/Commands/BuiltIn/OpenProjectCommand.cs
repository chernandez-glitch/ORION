using Orion.Application.Memory;
using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre un proyecto favorito recordado en memoria (lo abre en VS Code).</summary>
public sealed class OpenProjectCommand(IMemoryService memory, IProcessAutomation process) : CommandBase
{
    public override string Id => "vscode.open-project";

    public override string Name => "Abrir proyecto";

    public override string Description => "Abre un proyecto favorito por su nombre recordado.";

    public override CommandCategory Category => CommandCategory.VSCode;

    public override IReadOnlyList<string> Aliases => ["proyecto", "project", "abrir-proyecto"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("name", "Nombre del proyecto favorito.", IsRequired: true, Example: "orion")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var name = context.GetParameter("name")!;

        var project = await memory.GetProjectByNameAsync(name, context.CancellationToken).ConfigureAwait(false);
        if (project.IsFailure)
        {
            return CommandResult.Failed(project.Error.Message);
        }

        var launch = await process.LaunchAsync("code", project.Value.Path, context.CancellationToken).ConfigureAwait(false);
        return launch.IsSuccess
            ? CommandResult.Success($"Abriendo el proyecto '{project.Value.Name}'.")
            : CommandResult.Failed(launch.Error.Message);
    }
}
