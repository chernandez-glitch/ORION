using Orion.Memory.Engine.Entities;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>
/// Repositorio genérico sobre cualquier entidad de memoria. Seam de acceso a
/// datos reutilizable (por servicios, tests y, en el futuro, la IA).
/// </summary>
public interface IMemoryRepository<T> where T : MemoryEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    IQueryable<T> Query();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
