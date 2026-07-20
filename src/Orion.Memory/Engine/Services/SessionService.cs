using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class SessionService(MemoryDbContext db, ILogger<SessionService> logger) : ISessionService
{
    public Task<Result<Session>> StartSessionAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "StartSession", async () =>
        {
            var session = new Session
            {
                StartedOnUtc = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                UserName = Environment.UserName
            };

            await db.Sessions.AddAsync(session, cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Sesión iniciada {SessionId}.", session.Id);
            return Result.Success(session);
        });

    public Task<Result> EndSessionAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "EndSession", async () =>
        {
            var session = await db.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken).ConfigureAwait(false);
            if (session is null)
            {
                return Result.Failure(Error.NotFound("Memory.SessionNotFound", "Sesión no encontrada."));
            }

            var now = DateTime.UtcNow;
            session.EndedOnUtc = now;
            session.DurationSeconds = (long)(now - session.StartedOnUtc).TotalSeconds;
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<IReadOnlyList<Session>>> GetRecentAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetRecentSessions", async () =>
        {
            IReadOnlyList<Session> sessions = await db.Sessions
                .OrderByDescending(s => s.StartedOnUtc)
                .Take(take)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return Result.Success(sessions);
        });

    public Task<Result<int>> CountAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "CountSessions", async () =>
            Result.Success(await db.Sessions.CountAsync(cancellationToken).ConfigureAwait(false)));
}
