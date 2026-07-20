using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class ProjectMemoryService(MemoryDbContext db, ILogger<ProjectMemoryService> logger) : IProjectMemoryService
{
    public Task<Result<Project>> RememberAsync(string name, string path, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RememberProject", async () =>
        {
            var normalized = path.Trim();
            var project = await db.Projects.FirstOrDefaultAsync(p => p.Path == normalized, cancellationToken).ConfigureAwait(false);

            if (project is null)
            {
                project = new Project { Name = name.Trim(), Path = normalized, OpenCount = 1, LastOpenedOnUtc = DateTime.UtcNow };
                await db.Projects.AddAsync(project, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                project.OpenCount++;
                project.LastOpenedOnUtc = DateTime.UtcNow;
                project.Name = name.Trim();
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(project);
        });

    public Task<Result<IReadOnlyList<Project>>> GetRecentAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetRecentProjects", async () =>
        {
            IReadOnlyList<Project> projects = await db.Projects
                .OrderByDescending(p => p.LastOpenedOnUtc)
                .Take(take)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return Result.Success(projects);
        });

    public Task<Result<int>> CountAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "CountProjects", async () =>
            Result.Success(await db.Projects.CountAsync(cancellationToken).ConfigureAwait(false)));
}
