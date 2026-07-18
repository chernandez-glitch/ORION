namespace Orion.Domain.Common;

/// <summary>
/// Confirma de forma atómica todos los cambios pendientes de las entidades del
/// dominio. Implementado por la capa de infraestructura (EF Core).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
