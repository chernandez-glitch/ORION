using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Routes;

/// <summary>
/// Ruta favorita con alias (p. ej. "descargas" → C:\Users\...\Downloads).
/// Permite a ORION resolver ubicaciones por nombre natural.
/// </summary>
public sealed class FavoriteRoute : AuditableEntity
{
    private FavoriteRoute(Guid id, Guid userId, string alias, string path, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        UserId = userId;
        Alias = alias;
        Path = path;
    }

    public Guid UserId { get; private init; }

    public string Alias { get; private set; }

    public string Path { get; private set; }

    public static FavoriteRoute Create(Guid userId, string alias, string path, DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(alias);
        Guard.AgainstNullOrWhiteSpace(path);

        return new FavoriteRoute(Guid.CreateVersion7(), userId, alias.Trim(), path.Trim(), nowUtc);
    }

    public void Update(string alias, string path, DateTime nowUtc)
    {
        Alias = Guard.AgainstNullOrWhiteSpace(alias).Trim();
        Path = Guard.AgainstNullOrWhiteSpace(path).Trim();
        Touch(nowUtc);
    }
}
