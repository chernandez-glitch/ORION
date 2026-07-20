using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Gestiona las sesiones de uso de ORION.</summary>
public interface ISessionService
{
    Task<Result<Session>> StartSessionAsync(CancellationToken cancellationToken = default);

    Task<Result> EndSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Session>>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<int>> CountAsync(CancellationToken cancellationToken = default);
}
