using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

/// <summary>Exporta la memoria a JSON (completo) o CSV (por tabla) y la restaura desde JSON.</summary>
internal sealed class JsonCsvMemoryExportService(MemoryDbContext db, ILogger<JsonCsvMemoryExportService> logger) : IMemoryExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter() }
    };

    public Task<Result<string>> ExportJsonAsync(string filePath, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "ExportJson", async () =>
        {
            var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);
            EnsureDirectory(filePath);
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(snapshot, JsonOptions), cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Memoria exportada a JSON: {Path}", filePath);
            return Result.Success(filePath);
        });

    public Task<Result<string>> ExportCsvAsync(string directoryPath, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "ExportCsv", async () =>
        {
            Directory.CreateDirectory(directoryPath);
            var snapshot = await LoadSnapshotAsync(cancellationToken).ConfigureAwait(false);

            await WriteCsvAsync(Path.Combine(directoryPath, "commands.csv"),
                ["Id", "CommandId", "CommandName", "Status", "DurationMs", "ExecutedOnUtc"],
                snapshot.CommandHistory.Select(c => new[] { c.Id.ToString(), c.CommandId, c.CommandName, c.Status, c.DurationMs.ToString(CultureInfo.InvariantCulture), c.ExecutedOnUtc.ToString("o") }),
                cancellationToken).ConfigureAwait(false);

            await WriteCsvAsync(Path.Combine(directoryPath, "applications.csv"),
                ["Application", "LaunchCount", "LastLaunchedOnUtc"],
                snapshot.ApplicationHistory.Select(a => new[] { a.Application, a.LaunchCount.ToString(CultureInfo.InvariantCulture), a.LastLaunchedOnUtc.ToString("o") }),
                cancellationToken).ConfigureAwait(false);

            await WriteCsvAsync(Path.Combine(directoryPath, "projects.csv"),
                ["Name", "Path", "OpenCount", "LastOpenedOnUtc"],
                snapshot.Projects.Select(p => new[] { p.Name, p.Path, p.OpenCount.ToString(CultureInfo.InvariantCulture), p.LastOpenedOnUtc.ToString("o") }),
                cancellationToken).ConfigureAwait(false);

            await WriteCsvAsync(Path.Combine(directoryPath, "recent-files.csv"),
                ["Name", "Path", "AccessedOnUtc"],
                snapshot.RecentFiles.Select(f => new[] { f.Name, f.Path, f.AccessedOnUtc.ToString("o") }),
                cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Memoria exportada a CSV en: {Directory}", directoryPath);
            return Result.Success(directoryPath);
        });

    public Task<Result> ImportJsonAsync(string filePath, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "ImportJson", async () =>
        {
            if (!File.Exists(filePath))
            {
                return Result.Failure(Error.NotFound("Memory.ImportNotFound", $"No existe el archivo '{filePath}'."));
            }

            var json = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
            var snapshot = JsonSerializer.Deserialize<MemorySnapshot>(json, JsonOptions);
            if (snapshot is null)
            {
                return Result.Failure(Error.Failure("Memory.ImportInvalid", "El archivo de memoria no es válido."));
            }

            await MergeAsync(db.Sessions, snapshot.Sessions, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.Projects, snapshot.Projects, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.Conversations, snapshot.Conversations, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.ConversationMessages, snapshot.ConversationMessages, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.Favorites, snapshot.Favorites, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.CommandHistory, snapshot.CommandHistory, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.ApplicationHistory, snapshot.ApplicationHistory, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.FolderHistory, snapshot.FolderHistory, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.RecentFiles, snapshot.RecentFiles, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.History, snapshot.History, cancellationToken).ConfigureAwait(false);
            await MergeAsync(db.SettingSnapshots, snapshot.SettingSnapshots, cancellationToken).ConfigureAwait(false);

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Memoria importada desde: {Path}", filePath);
            return Result.Success();
        });

    private async Task<MemorySnapshot> LoadSnapshotAsync(CancellationToken cancellationToken) => new()
    {
        Sessions = await db.Sessions.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        Conversations = await db.Conversations.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        ConversationMessages = await db.ConversationMessages.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        Projects = await db.Projects.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        Favorites = await db.Favorites.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        History = await db.History.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        CommandHistory = await db.CommandHistory.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        ApplicationHistory = await db.ApplicationHistory.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        FolderHistory = await db.FolderHistory.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        RecentFiles = await db.RecentFiles.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false),
        SettingSnapshots = await db.SettingSnapshots.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false)
    };

    private static async Task MergeAsync<T>(DbSet<T> set, List<T> incoming, CancellationToken cancellationToken) where T : MemoryEntity
    {
        var existing = await set.Select(e => e.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
        var known = existing.ToHashSet();
        foreach (var entity in incoming.Where(e => !known.Contains(e.Id)))
        {
            await set.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task WriteCsvAsync(string path, string[] headers, IEnumerable<string[]> rows, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', headers.Select(Escape)));
        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(',', row.Select(Escape)));
        }

        await File.WriteAllTextAsync(path, sb.ToString(), cancellationToken).ConfigureAwait(false);
    }

    private static string Escape(string value)
    {
        value ??= string.Empty;
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }

    private static void EnsureDirectory(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>Estructura plana serializable de toda la memoria.</summary>
    private sealed class MemorySnapshot
    {
        public List<Session> Sessions { get; set; } = [];
        public List<Conversation> Conversations { get; set; } = [];
        public List<ConversationMessage> ConversationMessages { get; set; } = [];
        public List<Project> Projects { get; set; } = [];
        public List<Favorite> Favorites { get; set; } = [];
        public List<History> History { get; set; } = [];
        public List<CommandHistory> CommandHistory { get; set; } = [];
        public List<ApplicationHistory> ApplicationHistory { get; set; } = [];
        public List<FolderHistory> FolderHistory { get; set; } = [];
        public List<RecentFile> RecentFiles { get; set; } = [];
        public List<SettingSnapshot> SettingSnapshots { get; set; } = [];
    }
}
