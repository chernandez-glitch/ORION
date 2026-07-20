using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Registra y consulta el historial: comandos, aplicaciones, carpetas, archivos y eventos.</summary>
public interface IHistoryService
{
    Task<Result> RecordCommandAsync(string commandId, string commandName, string parameters, string status, bool succeeded, long durationMs, Guid? sessionId, CancellationToken cancellationToken = default);

    Task<Result> RecordApplicationAsync(string application, string? path, CancellationToken cancellationToken = default);

    Task<Result> RecordFolderAsync(string path, CancellationToken cancellationToken = default);

    Task<Result> RecordRecentFileAsync(string path, CancellationToken cancellationToken = default);

    Task<Result> RecordEventAsync(string category, string description, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<CommandHistory>>> GetRecentCommandsAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ApplicationHistory>>> GetApplicationsAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<FolderHistory>>> GetFoldersAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<RecentFile>>> GetRecentFilesAsync(int take, CancellationToken cancellationToken = default);
}
