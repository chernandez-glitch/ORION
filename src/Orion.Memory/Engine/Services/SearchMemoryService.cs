using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

/// <summary>
/// Búsqueda estructurada sobre toda la memoria (sin IA). Filtra por texto, fecha,
/// proyecto, etiqueta, tipo y categoría, y unifica los resultados.
/// </summary>
internal sealed class SearchMemoryService(MemoryDbContext db, ILogger<SearchMemoryService> logger) : ISearchMemoryService
{
    private const int PerSource = 20;
    private const int MaxResults = 60;

    public Task<Result<IReadOnlyList<MemorySearchResult>>> SearchAsync(MemoryQuery query, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "Search", async () =>
        {
            var text = query.Text?.Trim();
            var from = query.From;
            var to = query.To;
            var results = new List<MemorySearchResult>();

            var conversations = db.Conversations.AsQueryable();
            if (!string.IsNullOrEmpty(text)) conversations = conversations.Where(c => c.Title.Contains(text));
            if (from is not null) conversations = conversations.Where(c => c.UpdatedOnUtc >= from);
            if (to is not null) conversations = conversations.Where(c => c.UpdatedOnUtc <= to);
            results.AddRange((await conversations.OrderByDescending(c => c.UpdatedOnUtc).Take(PerSource).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(c => new MemorySearchResult("Conversación", c.Title, string.Empty, c.UpdatedOnUtc)));

            var commands = db.CommandHistory.AsQueryable();
            if (!string.IsNullOrEmpty(text)) commands = commands.Where(c => c.CommandName.Contains(text) || c.CommandId.Contains(text));
            if (from is not null) commands = commands.Where(c => c.ExecutedOnUtc >= from);
            if (to is not null) commands = commands.Where(c => c.ExecutedOnUtc <= to);
            results.AddRange((await commands.OrderByDescending(c => c.ExecutedOnUtc).Take(PerSource).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(c => new MemorySearchResult("Comando", c.CommandName, c.Status, c.ExecutedOnUtc)));

            var items = db.MemoryItems.AsQueryable();
            if (!string.IsNullOrEmpty(text)) items = items.Where(m => m.Title.Contains(text) || m.Content.Contains(text));
            if (from is not null) items = items.Where(m => m.CreatedOnUtc >= from);
            if (to is not null) items = items.Where(m => m.CreatedOnUtc <= to);
            if (!string.IsNullOrWhiteSpace(query.Category)) items = items.Where(m => m.Category == query.Category);
            if (Enum.TryParse<MemoryItemType>(query.Type, ignoreCase: true, out var type)) items = items.Where(m => m.Type == type);
            if (!string.IsNullOrWhiteSpace(query.Tag)) items = items.Where(m => m.Tags.Any(t => t.Name == query.Tag));
            results.AddRange((await items.OrderByDescending(m => m.CreatedOnUtc).Take(PerSource).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(m => new MemorySearchResult("Nota", m.Title, Snippet(m.Content), m.CreatedOnUtc)));

            var files = db.RecentFiles.AsQueryable();
            if (!string.IsNullOrEmpty(text)) files = files.Where(f => f.Name.Contains(text) || f.Path.Contains(text));
            if (from is not null) files = files.Where(f => f.AccessedOnUtc >= from);
            if (to is not null) files = files.Where(f => f.AccessedOnUtc <= to);
            results.AddRange((await files.OrderByDescending(f => f.AccessedOnUtc).Take(PerSource).ToListAsync(cancellationToken).ConfigureAwait(false))
                .Select(f => new MemorySearchResult("Archivo", f.Name, f.Path, f.AccessedOnUtc)));

            var projectTerm = query.Project ?? text;
            if (!string.IsNullOrEmpty(projectTerm))
            {
                var projects = await db.Projects
                    .Where(p => p.Name.Contains(projectTerm) || p.Path.Contains(projectTerm))
                    .OrderByDescending(p => p.LastOpenedOnUtc).Take(PerSource)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                results.AddRange(projects.Select(p => new MemorySearchResult("Proyecto", p.Name, p.Path, p.LastOpenedOnUtc)));
            }

            IReadOnlyList<MemorySearchResult> ordered = results
                .OrderByDescending(r => r.OccurredOnUtc)
                .Take(MaxResults)
                .ToArray();

            return Result.Success(ordered);
        });

    private static string Snippet(string content) =>
        content.Length <= 120 ? content : content[..120] + "…";
}
