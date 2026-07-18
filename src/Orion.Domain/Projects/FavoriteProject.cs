using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Projects;

/// <summary>
/// Proyecto favorito del usuario (una carpeta de trabajo, típicamente un repo).
/// ORION lo recuerda para abrirlo o retomar contexto rápidamente.
/// </summary>
public sealed class FavoriteProject : AuditableEntity
{
    private FavoriteProject(Guid id, Guid userId, string name, string path, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        UserId = userId;
        Name = name;
        Path = path;
    }

    public Guid UserId { get; private init; }

    public string Name { get; private set; }

    public string Path { get; private set; }

    public DateTime? LastOpenedOnUtc { get; private set; }

    public static FavoriteProject Create(Guid userId, string name, string path, DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(name);
        Guard.AgainstNullOrWhiteSpace(path);

        return new FavoriteProject(Guid.CreateVersion7(), userId, name.Trim(), path.Trim(), nowUtc);
    }

    public void MarkOpened(DateTime nowUtc)
    {
        LastOpenedOnUtc = nowUtc;
        Touch(nowUtc);
    }

    public void Update(string name, string path, DateTime nowUtc)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name).Trim();
        Path = Guard.AgainstNullOrWhiteSpace(path).Trim();
        Touch(nowUtc);
    }
}
