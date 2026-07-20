using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class HistoryService(MemoryDbContext db, ILogger<HistoryService> logger) : IHistoryService
{
    public Task<Result> RecordCommandAsync(string commandId, string commandName, string parameters, string status, bool succeeded, long durationMs, Guid? sessionId, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RecordCommand", async () =>
        {
            db.CommandHistory.Add(new CommandHistory
            {
                CommandId = commandId,
                CommandName = commandName,
                Parameters = parameters,
                Status = status,
                Succeeded = succeeded,
                DurationMs = durationMs,
                ExecutedOnUtc = DateTime.UtcNow,
                SessionId = sessionId
            });

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result> RecordApplicationAsync(string application, string? path, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RecordApplication", async () =>
        {
            var name = application.Trim();
            var existing = await db.ApplicationHistory.FirstOrDefaultAsync(a => a.Application == name, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                db.ApplicationHistory.Add(new ApplicationHistory { Application = name, Path = path, LaunchCount = 1, LastLaunchedOnUtc = DateTime.UtcNow });
            }
            else
            {
                existing.LaunchCount++;
                existing.LastLaunchedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result> RecordFolderAsync(string path, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RecordFolder", async () =>
        {
            var normalized = path.Trim();
            var existing = await db.FolderHistory.FirstOrDefaultAsync(f => f.Path == normalized, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                db.FolderHistory.Add(new FolderHistory { Path = normalized, OpenCount = 1, LastOpenedOnUtc = DateTime.UtcNow });
            }
            else
            {
                existing.OpenCount++;
                existing.LastOpenedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result> RecordRecentFileAsync(string path, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RecordRecentFile", async () =>
        {
            var normalized = path.Trim();
            var existing = await db.RecentFiles.FirstOrDefaultAsync(f => f.Path == normalized, cancellationToken).ConfigureAwait(false);
            if (existing is null)
            {
                db.RecentFiles.Add(new RecentFile { Path = normalized, Name = System.IO.Path.GetFileName(normalized), AccessedOnUtc = DateTime.UtcNow });
            }
            else
            {
                existing.AccessedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result> RecordEventAsync(string category, string description, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RecordEvent", async () =>
        {
            db.History.Add(new History { Category = category, Description = description, OccurredOnUtc = DateTime.UtcNow });
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<IReadOnlyList<CommandHistory>>> GetRecentCommandsAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetRecentCommands", async () =>
        {
            IReadOnlyList<CommandHistory> items = await db.CommandHistory
                .OrderByDescending(c => c.ExecutedOnUtc).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(items);
        });

    public Task<Result<IReadOnlyList<ApplicationHistory>>> GetApplicationsAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetApplications", async () =>
        {
            IReadOnlyList<ApplicationHistory> items = await db.ApplicationHistory
                .OrderByDescending(a => a.LaunchCount).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(items);
        });

    public Task<Result<IReadOnlyList<FolderHistory>>> GetFoldersAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetFolders", async () =>
        {
            IReadOnlyList<FolderHistory> items = await db.FolderHistory
                .OrderByDescending(f => f.LastOpenedOnUtc).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(items);
        });

    public Task<Result<IReadOnlyList<RecentFile>>> GetRecentFilesAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetRecentFiles", async () =>
        {
            IReadOnlyList<RecentFile> items = await db.RecentFiles
                .OrderByDescending(f => f.AccessedOnUtc).Take(take).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(items);
        });
}
