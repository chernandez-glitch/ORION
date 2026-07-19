namespace Orion.Application.Commands;

/// <summary>
/// Base de todo comando: reúne metadatos (<see cref="ICommand"/>) y ejecución
/// (<see cref="ICommandHandler"/>) en una sola clase. Agregar un comando nuevo
/// es heredar de aquí, declarar sus metadatos y escribir <c>ExecuteAsync</c>.
/// </summary>
public abstract class CommandBase : ICommand, ICommandHandler
{
    public abstract string Id { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract CommandCategory Category { get; }

    public virtual CommandPermission Permission => CommandPermission.User;

    public virtual IReadOnlyList<string> Aliases => [];

    public virtual IReadOnlyList<CommandParameter> Parameters => [];

    public abstract Task<CommandResult> ExecuteAsync(ICommandContext context);

    /// <summary>Id + alias: todas las claves con las que se puede invocar.</summary>
    public IEnumerable<string> Keys => new[] { Id }.Concat(Aliases);
}
