namespace Orion.Application.Abstractions;

/// <summary>
/// Mantiene la identidad del usuario activo de ORION (aplicación de escritorio
/// mono-usuario por sesión). El sistema de memoria la usa para asociar datos.
/// </summary>
public interface IActiveUserContext
{
    /// <summary>Id del usuario activo, o <see cref="Guid.Empty"/> si aún no se resolvió.</summary>
    Guid CurrentUserId { get; }

    bool HasUser { get; }

    void SetUser(Guid userId);
}
