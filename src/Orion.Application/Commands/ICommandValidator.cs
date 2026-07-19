namespace Orion.Application.Commands;

/// <summary>
/// Valida un comando antes de ejecutarlo. Devuelve <c>null</c> si es válido, o
/// un <see cref="CommandResult"/> fallido describiendo el problema.
/// </summary>
public interface ICommandValidator
{
    CommandResult? Validate(CommandInfo command, ICommandContext context);
}
