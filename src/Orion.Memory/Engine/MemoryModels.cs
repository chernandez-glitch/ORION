using Orion.Memory.Engine.Entities;

namespace Orion.Memory.Engine;

/// <summary>Criterios de búsqueda en la memoria (texto, proyecto, fechas, etiqueta, tipo, categoría).</summary>
public sealed record MemoryQuery(
    string? Text = null,
    string? Project = null,
    DateTime? From = null,
    DateTime? To = null,
    string? Tag = null,
    string? Type = null,
    string? Category = null);

/// <summary>Resultado unificado de búsqueda en la memoria.</summary>
public sealed record MemorySearchResult(string Kind, string Title, string Snippet, DateTime OccurredOnUtc);

/// <summary>
/// Instantánea de contexto que la IA consultará: sesión actual y lo más reciente
/// de cada área de memoria.
/// </summary>
public sealed record MemoryContext(
    Session? CurrentSession,
    IReadOnlyList<CommandHistory> RecentCommands,
    IReadOnlyList<Project> RecentProjects,
    IReadOnlyList<ApplicationHistory> FrequentApplications,
    IReadOnlyList<RecentFile> RecentFiles,
    IReadOnlyList<Conversation> RecentConversations);
