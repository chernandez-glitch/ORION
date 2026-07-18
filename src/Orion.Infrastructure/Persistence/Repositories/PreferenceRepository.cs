using Microsoft.EntityFrameworkCore;
using Orion.Domain.Preferences;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class PreferenceRepository(OrionDbContext db) : IPreferenceRepository
{
    public Task<Preference?> GetAsync(Guid userId, string key, CancellationToken cancellationToken = default) =>
        db.Preferences.FirstOrDefaultAsync(p => p.UserId == userId && p.Key == key, cancellationToken);

    public async Task<IReadOnlyList<Preference>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await db.Preferences.Where(p => p.UserId == userId).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task AddAsync(Preference preference, CancellationToken cancellationToken = default) =>
        await db.Preferences.AddAsync(preference, cancellationToken).ConfigureAwait(false);

    public void Update(Preference preference) => db.Preferences.Update(preference);

    public void Remove(Preference preference) => db.Preferences.Remove(preference);
}
