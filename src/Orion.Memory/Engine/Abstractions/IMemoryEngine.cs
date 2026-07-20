namespace Orion.Memory.Engine.Abstractions;

/// <summary>
/// Fachada del Memory Engine: reúne todos los servicios de memoria en un único
/// punto de acceso. (El <c>IMemoryService</c> de Fase 0 sigue existiendo aparte,
/// dando historial al Command Engine; esta fachada es la memoria permanente rica.)
/// </summary>
public interface IMemoryEngine
{
    ISessionService Sessions { get; }

    IConversationService Conversations { get; }

    IProjectMemoryService Projects { get; }

    IHistoryService History { get; }

    IFavoriteService Favorites { get; }

    ITagService Tags { get; }

    ISearchMemoryService Search { get; }

    IContextService Context { get; }

    IMemoryExportService Export { get; }
}
