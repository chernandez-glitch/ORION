namespace Orion.Domain.Common;

/// <summary>
/// Marca un evento de dominio: algo relevante que ocurrió y sobre lo que otras
/// partes del sistema pueden reaccionar (persistencia, notificaciones, etc.).
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredOnUtc { get; }
}
