using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una ventana de PowerShell.</summary>
public sealed class OpenPowerShellCommand(IProcessService process) : CommandBase
{
    public override string Id => "powershell.open";

    public override string Name => "Abrir PowerShell";

    public override string Description => "Abre una ventana de PowerShell.";

    public override CommandCategory Category => CommandCategory.PowerShell;

    public override IReadOnlyList<string> Aliases => ["powershell", "abrir-powershell", "ps-window"];

    public override Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var result = process.Start("powershell.exe");
        return Task.FromResult(result.IsSuccess
            ? CommandResult.Success("PowerShell abierto.")
            : CommandResult.Failed(result.Error.Message));
    }
}
