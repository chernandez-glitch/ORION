namespace Orion.Application.Commands;

/// <summary>
/// Autorizador por defecto para la app de escritorio mono-usuario: el usuario es
/// el dueño del equipo, así que permite todos los niveles. Aquí se enchufarán
/// las políticas de elevación/roles en fases posteriores.
/// </summary>
public sealed class DefaultCommandAuthorizer : ICommandAuthorizer
{
    public CommandResult? Authorize(CommandInfo command, ICommandContext context) => null;
}
