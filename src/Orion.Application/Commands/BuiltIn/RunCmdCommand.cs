using Orion.Windows.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Ejecuta un comando de CMD y devuelve su respuesta.</summary>
public sealed class RunCmdCommand(ICmdService cmd) : CommandBase
{
    public override string Id => "cmd.run";

    public override string Name => "Ejecutar CMD";

    public override string Description => "Ejecuta un comando de CMD y captura su respuesta.";

    public override CommandCategory Category => CommandCategory.System;

    public override CommandPermission Permission => CommandPermission.Elevated;

    public override IReadOnlyList<string> Aliases => ["cmd", "ejecutar-cmd", "run-cmd"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("command", "Comando a ejecutar.", IsRequired: true, Example: "ipconfig")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var command = context.GetParameter("command")!;
        var result = await cmd.RunAsync(command, context.CancellationToken).ConfigureAwait(false);

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
