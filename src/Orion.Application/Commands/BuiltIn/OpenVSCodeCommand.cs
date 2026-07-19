using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre Visual Studio Code, opcionalmente en una carpeta.</summary>
public sealed class OpenVSCodeCommand(IProcessAutomation process) : CommandBase
{
    public override string Id => "vscode.open";

    public override string Name => "Abrir VS Code";

    public override string Description => "Abre Visual Studio Code en la carpeta indicada (o en la actual).";

    public override CommandCategory Category => CommandCategory.VSCode;

    public override IReadOnlyList<string> Aliases => ["vscode", "code", "abrir vscode", "abrir codigo"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("path", "Carpeta a abrir.", IsRequired: false, Example: @"C:\repos\orion")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var path = context.HasParameter("path") ? context.GetParameter("path")! : ".";
        var result = await process.LaunchAsync("code", path, context.CancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? CommandResult.Success($"Abriendo VS Code en '{path}'.")
            : CommandResult.Failed(result.Error.Message);
    }
}
