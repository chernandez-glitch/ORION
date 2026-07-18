using Orion.Application.Memory.Dtos;
using Orion.Domain.Conversations;
using Orion.Shared.Results;

namespace Orion.Application.Memory;

/// <summary>
/// Fachada del sistema de memoria de ORION. Concentra el acceso a lo que el
/// asistente "recuerda": usuario activo, preferencias, proyectos, rutas,
/// historial de comandos y conversaciones. Implementado por Orion.Memory.
/// </summary>
public interface IMemoryService
{
    /// <summary>Garantiza que exista un usuario activo (crea uno por defecto si hace falta).</summary>
    Task<Result<Guid>> GetOrCreateActiveUserAsync(CancellationToken cancellationToken = default);

    // ----- Preferencias -----
    Task<Result<string?>> GetPreferenceAsync(string key, CancellationToken cancellationToken = default);

    Task<Result> SetPreferenceAsync(string key, string value, CancellationToken cancellationToken = default);

    // ----- Proyectos favoritos -----
    Task<Result<IReadOnlyList<FavoriteProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default);

    Task<Result<FavoriteProjectDto>> AddProjectAsync(string name, string path, CancellationToken cancellationToken = default);

    Task<Result<FavoriteProjectDto>> GetProjectByNameAsync(string name, CancellationToken cancellationToken = default);

    // ----- Rutas favoritas -----
    Task<Result<IReadOnlyList<FavoriteRouteDto>>> GetRoutesAsync(CancellationToken cancellationToken = default);

    Task<Result<FavoriteRouteDto>> AddRouteAsync(string alias, string path, CancellationToken cancellationToken = default);

    Task<Result<string>> ResolveRouteAsync(string alias, CancellationToken cancellationToken = default);

    // ----- Historial de comandos -----
    Task<Result> RecordCommandAsync(
        string commandName,
        string rawInput,
        bool succeeded,
        string outcome,
        long durationMs,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<CommandHistoryDto>>> GetRecentCommandsAsync(int take, CancellationToken cancellationToken = default);

    // ----- Conversaciones -----
    Task<Result<ConversationDto>> StartConversationAsync(string title, CancellationToken cancellationToken = default);

    Task<Result> AddMessageAsync(Guid conversationId, ConversationRole role, string content, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ConversationDto>>> GetRecentConversationsAsync(int take, CancellationToken cancellationToken = default);
}
