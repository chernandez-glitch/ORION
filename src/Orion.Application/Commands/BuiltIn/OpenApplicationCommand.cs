using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una aplicación por ruta o nombre de ejecutable (notepad, calc, Code…).</summary>
public sealed class OpenApplicationCommand(IProcessAutomation process) : CommandBase
{
    public override string Id => "apps.open";

    public override string Name => "Abrir aplicación";

    public override string Description => "Abre una aplicación por su nombre o ruta (p. ej. notepad, calc, Code).";

    public override CommandCategory Category => CommandCategory.Applications;

    public override IReadOnlyList<string> Aliases => ["abrir-app", "abrir aplicacion", "open app", "run", "ejecutar"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("app", "Nombre o ruta del ejecutable.", IsRequired: true, Example: "notepad")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var app = context.GetParameter("app")!;
        var result = await process.LaunchAsync(app, arguments: null, context.CancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? CommandResult.Success($"Abriendo '{app}'.", result.Value)
            : CommandResult.Failed(result.Error.Message);
    }
}
