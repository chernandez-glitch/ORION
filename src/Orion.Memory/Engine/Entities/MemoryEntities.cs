namespace Orion.Memory.Engine.Entities;

/// <summary>Tipo de elemento marcable como favorito.</summary>
public enum FavoriteKind
{
    Command = 0,
    Folder = 1,
    Application = 2,
    Project = 3,
    Conversation = 4
}

/// <summary>Autor de un mensaje en una conversación recordada.</summary>
public enum MemoryMessageRole
{
    System = 0,
    User = 1,
    Assistant = 2
}

/// <summary>Naturaleza de un elemento de memoria genérico.</summary>
public enum MemoryItemType
{
    Note = 0,
    Context = 1,
    Snippet = 2,
    Other = 3
}

/// <summary>Base de toda entidad de memoria: identidad + marca de creación (UTC).</summary>
public abstract class MemoryEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>Sesión de uso de ORION (desde el arranque hasta el cierre).</summary>
public sealed class Session : MemoryEntity
{
    public DateTime StartedOnUtc { get; set; }

    public DateTime? EndedOnUtc { get; set; }

    public long DurationSeconds { get; set; }

    public int EventCount { get; set; }

    public string MachineName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}

/// <summary>Hilo de conversación recordado.</summary>
public sealed class Conversation : MemoryEntity
{
    public string Title { get; set; } = string.Empty;

    public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;

    public Guid? SessionId { get; set; }

    public List<ConversationMessage> Messages { get; set; } = [];
}

public sealed class ConversationMessage : MemoryEntity
{
    public Guid ConversationId { get; set; }

    public MemoryMessageRole Role { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>Proyecto recordado (carpeta de trabajo).</summary>
public sealed class Project : MemoryEntity
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public DateTime LastOpenedOnUtc { get; set; } = DateTime.UtcNow;

    public int OpenCount { get; set; }
}

/// <summary>Espacio de trabajo (agrupación lógica).</summary>
public sealed class Workspace : MemoryEntity
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
}

/// <summary>Elemento marcado como favorito.</summary>
public sealed class Favorite : MemoryEntity
{
    public FavoriteKind Kind { get; set; }

    /// <summary>Clave/id/ruta del elemento referenciado.</summary>
    public string Reference { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;
}

/// <summary>Evento genérico del historial.</summary>
public sealed class History : MemoryEntity
{
    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>Comando ejecutado (historial del Command Engine).</summary>
public sealed class CommandHistory : MemoryEntity
{
    public string CommandId { get; set; } = string.Empty;

    public string CommandName { get; set; } = string.Empty;

    public string Parameters { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool Succeeded { get; set; }

    public long DurationMs { get; set; }

    public DateTime ExecutedOnUtc { get; set; } = DateTime.UtcNow;

    public Guid? SessionId { get; set; }
}

/// <summary>Aplicación abierta (frecuencia de uso).</summary>
public sealed class ApplicationHistory : MemoryEntity
{
    public string Application { get; set; } = string.Empty;

    public string? Path { get; set; }

    public DateTime LastLaunchedOnUtc { get; set; } = DateTime.UtcNow;

    public int LaunchCount { get; set; }
}

/// <summary>Carpeta abierta (frecuencia de uso).</summary>
public sealed class FolderHistory : MemoryEntity
{
    public string Path { get; set; } = string.Empty;

    public DateTime LastOpenedOnUtc { get; set; } = DateTime.UtcNow;

    public int OpenCount { get; set; }
}

/// <summary>Archivo reciente.</summary>
public sealed class RecentFile : MemoryEntity
{
    public string Path { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime AccessedOnUtc { get; set; } = DateTime.UtcNow;
}

/// <summary>Etiqueta para clasificar elementos de memoria.</summary>
public sealed class Tag : MemoryEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public List<MemoryItem> Items { get; set; } = [];
}

/// <summary>Elemento de memoria genérico (nota, contexto, fragmento), etiquetable.</summary>
public sealed class MemoryItem : MemoryEntity
{
    public MemoryItemType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<Tag> Tags { get; set; } = [];
}

/// <summary>Instantánea de configuración (para restaurar ajustes).</summary>
public sealed class SettingSnapshot : MemoryEntity
{
    public string Label { get; set; } = string.Empty;

    public string Json { get; set; } = string.Empty;
}
