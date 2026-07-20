using Orion.Memory.Engine.Abstractions;

namespace Orion.Memory.Engine.Services;

/// <summary>Fachada que reúne todos los servicios del Memory Engine.</summary>
internal sealed class MemoryEngine(
    ISessionService sessions,
    IConversationService conversations,
    IProjectMemoryService projects,
    IHistoryService history,
    IFavoriteService favorites,
    ITagService tags,
    ISearchMemoryService search,
    IContextService context,
    IMemoryExportService export) : IMemoryEngine
{
    public ISessionService Sessions { get; } = sessions;

    public IConversationService Conversations { get; } = conversations;

    public IProjectMemoryService Projects { get; } = projects;

    public IHistoryService History { get; } = history;

    public IFavoriteService Favorites { get; } = favorites;

    public ITagService Tags { get; } = tags;

    public ISearchMemoryService Search { get; } = search;

    public IContextService Context { get; } = context;

    public IMemoryExportService Export { get; } = export;
}
