namespace Orion.Application.Abstractions;

/// <summary>Implementación en memoria del contexto de usuario activo.</summary>
public sealed class ActiveUserContext : IActiveUserContext
{
    private Guid _currentUserId;

    public Guid CurrentUserId => _currentUserId;

    public bool HasUser => _currentUserId != Guid.Empty;

    public void SetUser(Guid userId) => _currentUserId = userId;
}
