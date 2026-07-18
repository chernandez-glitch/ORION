using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Users;

/// <summary>
/// Persona que usa ORION. Es la raíz a la que se asocian preferencias,
/// proyectos, rutas, conversaciones e historial (el "quién recuerda" del
/// sistema de memoria).
/// </summary>
public sealed class OrionUser : AuditableEntity
{
    private OrionUser(Guid id, string displayName, string culture, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        DisplayName = displayName;
        Culture = culture;
    }

    public string DisplayName { get; private set; }

    /// <summary>Cultura preferida, p. ej. "es-HN". Nunca vacía.</summary>
    public string Culture { get; private set; }

    public static OrionUser Create(string displayName, string culture, DateTime nowUtc)
    {
        Guard.AgainstNullOrWhiteSpace(displayName);
        Guard.AgainstNullOrWhiteSpace(culture);

        return new OrionUser(Guid.CreateVersion7(), displayName.Trim(), culture.Trim(), nowUtc);
    }

    public void Rename(string displayName, DateTime nowUtc)
    {
        DisplayName = Guard.AgainstNullOrWhiteSpace(displayName).Trim();
        Touch(nowUtc);
    }

    public void ChangeCulture(string culture, DateTime nowUtc)
    {
        Culture = Guard.AgainstNullOrWhiteSpace(culture).Trim();
        Touch(nowUtc);
    }
}
