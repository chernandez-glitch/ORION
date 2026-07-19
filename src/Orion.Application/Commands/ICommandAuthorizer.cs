namespace Orion.Application.Commands;

/// <summary>
/// Autoriza la ejecución de un comando según su <see cref="CommandPermission"/>
/// y el contexto. Devuelve <c>null</c> si está permitido, o un resultado
/// denegado. Punto de extensión para políticas futuras (elevación, roles).
/// </summary>
public interface ICommandAuthorizer
{
    CommandResult? Authorize(CommandInfo command, ICommandContext context);
}
