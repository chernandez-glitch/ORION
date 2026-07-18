namespace Orion.Domain.Common;

/// <summary>
/// Entidad con marcas de auditoría temporales. Los tiempos se expresan en UTC.
/// </summary>
public abstract class AuditableEntity : Entity
{
    protected AuditableEntity(Guid id, DateTime createdOnUtc)
        : base(id)
    {
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    public DateTime CreatedOnUtc { get; protected set; }

    public DateTime UpdatedOnUtc { get; protected set; }

    protected void Touch(DateTime nowUtc) => UpdatedOnUtc = nowUtc;
}
