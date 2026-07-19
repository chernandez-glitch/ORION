namespace Orion.Application.Commands;

/// <summary>
/// Instantánea inmutable de los metadatos de un comando más el tipo CLR que lo
/// ejecuta. La registra el <see cref="ICommandRegistry"/> sin retener la
/// instancia viva del comando (que tiene dependencias con ámbito).
/// </summary>
public sealed record CommandInfo(
    string Id,
    string Name,
    string Description,
    CommandCategory Category,
    CommandPermission Permission,
    IReadOnlyList<string> Aliases,
    IReadOnlyList<CommandParameter> Parameters,
    Type HandlerType) : ICommand
{
    public IEnumerable<string> Keys => new[] { Id }.Concat(Aliases);

    /// <summary>Clave preferida para invocarlo (primer alias, o el Id).</summary>
    public string PrimaryKey => Aliases.Count > 0 ? Aliases[0] : Id;

    /// <summary>Coincidencia de búsqueda por nombre, id, descripción, categoría o alias.</summary>
    public bool Matches(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return true;
        }

        var t = term.Trim();
        return Name.Contains(t, StringComparison.OrdinalIgnoreCase)
            || Id.Contains(t, StringComparison.OrdinalIgnoreCase)
            || Description.Contains(t, StringComparison.OrdinalIgnoreCase)
            || Category.ToString().Contains(t, StringComparison.OrdinalIgnoreCase)
            || Aliases.Any(a => a.Contains(t, StringComparison.OrdinalIgnoreCase));
    }
}
