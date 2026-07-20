using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class TagService(MemoryDbContext db, ILogger<TagService> logger) : ITagService
{
    public Task<Result<Tag>> CreateAsync(string name, string? color = null, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "CreateTag", async () =>
        {
            var normalized = name.Trim();
            var existing = await db.Tags.FirstOrDefaultAsync(t => t.Name == normalized, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                return Result.Success(existing);
            }

            var tag = new Tag { Name = normalized, Color = color };
            await db.Tags.AddAsync(tag, cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(tag);
        });

    public Task<Result<IReadOnlyList<Tag>>> GetAllAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetTags", async () =>
        {
            IReadOnlyList<Tag> tags = await db.Tags.OrderBy(t => t.Name).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(tags);
        });

    public Task<Result> TagItemAsync(Guid memoryItemId, string tagName, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "TagItem", async () =>
        {
            var item = await db.MemoryItems.Include(m => m.Tags).FirstOrDefaultAsync(m => m.Id == memoryItemId, cancellationToken).ConfigureAwait(false);
            if (item is null)
            {
                return Result.Failure(Error.NotFound("Memory.ItemNotFound", "Elemento de memoria no encontrado."));
            }

            var normalized = tagName.Trim();
            var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == normalized, cancellationToken).ConfigureAwait(false)
                      ?? new Tag { Name = normalized };

            if (!item.Tags.Any(t => t.Name == normalized))
            {
                item.Tags.Add(tag);
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return Result.Success();
        });
}
