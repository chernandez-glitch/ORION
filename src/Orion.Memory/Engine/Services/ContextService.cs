using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

/// <summary>
/// Compone la instantánea de contexto actual a partir de lo más reciente de cada
/// área. Es lo que la IA consultará (Fase 2) para entender el trabajo en curso.
/// </summary>
internal sealed class ContextService(MemoryDbContext db, ILogger<ContextService> logger) : IContextService
{
    private const int Recent = 5;

    public Task<Result<MemoryContext>> GetCurrentContextAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetContext", async () =>
        {
            var session = await db.Sessions.OrderByDescending(s => s.StartedOnUtc).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            var commands = await db.CommandHistory.OrderByDescending(c => c.ExecutedOnUtc).Take(Recent).ToListAsync(cancellationToken).ConfigureAwait(false);
            var projects = await db.Projects.OrderByDescending(p => p.LastOpenedOnUtc).Take(Recent).ToListAsync(cancellationToken).ConfigureAwait(false);
            var apps = await db.ApplicationHistory.OrderByDescending(a => a.LaunchCount).Take(Recent).ToListAsync(cancellationToken).ConfigureAwait(false);
            var files = await db.RecentFiles.OrderByDescending(f => f.AccessedOnUtc).Take(Recent).ToListAsync(cancellationToken).ConfigureAwait(false);
            var conversations = await db.Conversations.OrderByDescending(c => c.UpdatedOnUtc).Take(Recent).ToListAsync(cancellationToken).ConfigureAwait(false);

            return Result.Success(new MemoryContext(session, commands, projects, apps, files, conversations));
        });
}
