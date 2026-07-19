namespace Orion.Application.Commands;

/// <summary>
/// Metadatos de un comando: su identidad, cómo se presenta, su categoría, el
/// permiso que exige, sus alias y sus parámetros. Cada comando concreto es una
/// clase independiente (normalmente heredando de <see cref="CommandBase"/>).
/// </summary>
public interface ICommand
{
    /// <summary>Identificador estable (p. ej. "apps.open"). No cambia entre versiones.</summary>
    string Id { get; }

    string Name { get; }

    string Description { get; }

    CommandCategory Category { get; }

    CommandPermission Permission { get; }

    /// <summary>Nombres alternativos para invocarlo/buscarlo.</summary>
    IReadOnlyList<string> Aliases { get; }

    IReadOnlyList<CommandParameter> Parameters { get; }
}
