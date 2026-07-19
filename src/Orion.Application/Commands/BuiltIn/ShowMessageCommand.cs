using Orion.Application.Commands.Abstractions;

namespace Orion.Application.Commands.BuiltIn;

/// <summary>Muestra una ventana moderna de información al usuario.</summary>
public sealed class ShowMessageCommand(IDialogService dialog) : CommandBase
{
    public override string Id => "system.show-message";

    public override string Name => "Mostrar mensaje";

    public override string Description => "Muestra una ventana de información con un título y un mensaje.";

    public override CommandCategory Category => CommandCategory.System;

    public override IReadOnlyList<string> Aliases => ["mensaje", "mostrar-mensaje", "show message", "msg"];

    public override IReadOnlyList<CommandParameter> Parameters =>
    [
        new CommandParameter("title", "Título de la ventana.", IsRequired: false, Example: "ORION"),
        new CommandParameter("message", "Texto a mostrar.", IsRequired: true, Example: "Hola desde ORION")
    ];

    public override async Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        var message = context.GetParameter("message")!;
        var title = context.HasParameter("title") ? context.GetParameter("title")! : "ORION AI";

        await dialog.ShowMessageAsync(title, message);
        return CommandResult.Success("Mensaje mostrado.");
    }
}
