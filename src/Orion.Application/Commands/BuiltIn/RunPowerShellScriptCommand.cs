using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Ejecuta un script de PowerShell y devuelve su salida.</summary>
public sealed class RunPowerShellScriptCommand(IPowerShellService powerShell) : CommandBase
{
    public override string Id => "powershell.run";

    public override string Name => "Ejecutar PowerShell";

    public override string Description => "Ejecuta un script de PowerShell y captura su salida.";

    public override CommandCategory Category => CommandCategory.PowerShell;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["ps", "ejecutar-powershell", "run-ps"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("script", "Script a ejecutar.", IsRequired: true, Example: "Get-Date")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var script = context.GetParameter("script")!;
        var result = await powerShell.RunScriptAsync(script, context.CancellationToken).ConfigureAwait(false);

        if (result.IsFailure)
        {
            return CommandResult.Failed(result.Error.Message);
        }

        var shell = result.Value;
        var output = string.IsNullOrWhiteSpace(shell.Output) ? "(sin salida)" : shell.Output;
        return shell.Succeeded
            ? CommandResult.Success(Truncate(output), shell)
            : CommandResult.Failed(string.IsNullOrWhiteSpace(shell.Error) ? output : shell.Error);
    }

    private static string Truncate(string text) =>
        text.Length <= 500 ? text : text[..500] + "…";
}
