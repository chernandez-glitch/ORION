using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Gestiona etiquetas y el etiquetado de elementos de memoria.</summary>
public interface ITagService
{
    Task<Result<Tag>> CreateAsync(string name, string? color = null, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Tag>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result> TagItemAsync(Guid memoryItemId, string tagName, CancellationToken cancellationToken = default);
}
