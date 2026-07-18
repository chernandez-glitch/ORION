namespace Orion.Domain.Projects;

public interface IFavoriteProjectRepository
{
    Task<FavoriteProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FavoriteProject>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(FavoriteProject project, CancellationToken cancellationToken = default);

    void Update(FavoriteProject project);

    void Remove(FavoriteProject project);
}
