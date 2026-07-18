namespace Orion.Application.Commands;

/// <summary>
/// Metadatos de un comando: su nombre canónico, alias, descripción y categoría.
/// El motor lo usa para resolver, listar y documentar comandos.
/// </summary>
public sealed record CommandDescriptor
{
    public CommandDescriptor(string name, string description, CommandCategory category, params string[] aliases)
    {
        Name = name;
        Description = description;
        Category = category;
        Aliases = aliases;
    }

    public string Name { get; }

    public string Description { get; }

    public CommandCategory Category { get; }

    public IReadOnlyList<string> Aliases { get; }

    /// <summary>Nombre canónico más todos sus alias.</summary>
    public IEnumerable<string> AllTokens => new[] { Name }.Concat(Aliases);
}
