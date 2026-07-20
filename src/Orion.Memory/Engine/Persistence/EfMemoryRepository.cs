using Microsoft.EntityFrameworkCore;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;

namespace Orion.Memory.Engine.Persistence;

/// <summary>Repositorio genérico respaldado por <see cref="MemoryDbContext"/>.</summary>
internal sealed class EfMemoryRepository<T>(MemoryDbContext context) : IMemoryRepository<T>
    where T : MemoryEntity
{
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Set<T>().ToListAsync(cancellationToken).ConfigureAwait(false);

    public IQueryable<T> Query() => context.Set<T>().AsQueryable();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);

    public void Update(T entity) => context.Set<T>().Update(entity);

    public void Remove(T entity) => context.Set<T>().Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
