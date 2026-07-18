using Microsoft.EntityFrameworkCore;
using Orion.Domain.Automations;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class AutomationRepository(OrionDbContext db) : IAutomationRepository
{
    public Task<AutomationDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Automations.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<AutomationDefinition>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await db.Automations
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(AutomationDefinition automation, CancellationToken cancellationToken = default) =>
        await db.Automations.AddAsync(automation, cancellationToken).ConfigureAwait(false);

    public void Update(AutomationDefinition automation) => db.Automations.Update(automation);

    public void Remove(AutomationDefinition automation) => db.Automations.Remove(automation);
}
