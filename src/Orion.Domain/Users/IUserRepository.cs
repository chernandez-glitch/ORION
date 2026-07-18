namespace Orion.Domain.Users;

public interface IUserRepository
{
    Task<OrionUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrionUser?> GetByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrionUser>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(OrionUser user, CancellationToken cancellationToken = default);

    void Update(OrionUser user);

    void Remove(OrionUser user);
}
