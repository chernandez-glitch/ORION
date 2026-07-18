using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Preferences;

/// <summary>
/// Preferencia arbitraria clave/valor asociada a un usuario. Permite a ORION
/// recordar ajustes y hábitos sin un esquema rígido.
/// </summary>
public sealed class Preference : AuditableEntity
{
    private Preference(Guid id, Guid userId, string key, string value, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        UserId = userId;
        Key = key;
        Value = value;
    }

    public Guid UserId { get; private init; }

    public string Key { get; private init; }

    public string Value { get; private set; }

    public static Preference Create(Guid userId, string key, string value, DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(key);

        return new Preference(Guid.CreateVersion7(), userId, key.Trim(), value ?? string.Empty, nowUtc);
    }

    public void UpdateValue(string value, DateTime nowUtc)
    {
        Value = value ?? string.Empty;
        Touch(nowUtc);
    }
}
