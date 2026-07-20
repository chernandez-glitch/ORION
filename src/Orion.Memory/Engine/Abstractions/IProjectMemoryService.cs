using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Recuerda proyectos abiertos (upsert con conteo de aperturas).</summary>
public interface IProjectMemoryService
{
    Task<Result<Project>> RememberAsync(string name, string path, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Project>>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<int>> CountAsync(CancellationToken cancellationToken = default);
}
