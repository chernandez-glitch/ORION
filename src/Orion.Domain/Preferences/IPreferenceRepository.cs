namespace Orion.Domain.Preferences;

public interface IPreferenceRepository
{
    Task<Preference?> GetAsync(Guid userId, string key, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Preference>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Preference preference, CancellationToken cancellationToken = default);

    void Update(Preference preference);

    void Remove(Preference preference);
}
