using Orion.Automation.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Abre una URL en el navegador predeterminado.</summary>
public sealed class OpenUrlCommand(IProcessAutomation process) : CommandBase
{
    public override string Id => "internet.open-url";

    public override string Name => "Abrir URL";

    public override string Description => "Abre una URL en el navegador predeterminado.";

    public override CommandCategory Category => CommandCategory.Internet;

    public override IReadOnlyList<string> Aliases => ["abrir-url", "url", "web", "navegador", "open url", "browser"];

    public override IReadOnlyList<CommandParameter> Parameters =>
        [new CommandParameter("url", "Dirección a abrir.", IsRequired: true, Example: "https://github.com")];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var url = context.GetParameter("url")!;
        var result = await process.LaunchAsync(url, arguments: null, context.CancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? CommandResult.Success($"Abriendo '{url}' en el navegador.")
            : CommandResult.Failed(result.Error.Message);
    }
}
