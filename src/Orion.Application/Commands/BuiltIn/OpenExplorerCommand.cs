using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre el Explorador de Windows (en "Este equipo").</summary>
public sealed class OpenExplorerCommand(IProcessAutomation process) : CommandBase
{
    public override string Id => "system.open-explorer";

    public override string Name => "Abrir Explorador";

    public override string Description => "Abre el Explorador de Windows.";

    public override CommandCategory Category => CommandCategory.System;

    public override IReadOnlyList<string> Aliases => ["explorador", "explorer", "open explorer", "archivos"];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = await process.LaunchAsync("explorer.exe", arguments: null, context.CancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? CommandResult.Success("Abriendo el Explorador de Windows.")
            : CommandResult.Failed(result.Error.Message);
    }
}
