namespace Orion.Configuration.Models;

/// <summary>
/// Raíz de la configuración persistente de ORION. Se serializa a JSON en el
/// perfil del usuario. Todas las secciones tienen valores por defecto sensatos.
/// </summary>
public sealed class OrionSettings
{
    public AssistantSettings Assistant { get; set; } = new();

    public AISettings AI { get; set; } = new();

    public VoiceSettings Voice { get; set; } = new();

    public AppearanceSettings Appearance { get; set; } = new();

    public PathsSettings Paths { get; set; } = new();

    /// <summary>Atajos globales: nombre lógico → combinación de teclas.</summary>
    public Dictionary<string, string> Shortcuts { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ToggleListening"] = "Ctrl+Shift+Space",
        ["OpenDashboard"] = "Ctrl+Shift+D",
        ["OpenSettings"] = "Ctrl+,"
    };

    public List<FavoriteRouteSettings> FavoriteRoutes { get; set; } = [];

    public List<FavoriteProjectSettings> FavoriteProjects { get; set; } = [];

    /// <summary>Ids de comandos marcados como favoritos en la Command Palette.</summary>
    public List<string> FavoriteCommandIds { get; set; } = [];
}

/// <summary>Identidad y activación del asistente.</summary>
public sealed class AssistantSettings
{
    public string Name { get; set; } = "Orion";

    public string ActivationWord { get; set; } = "Orion";

    public string Language { get; set; } = "es-HN";
}

/// <summary>Selección de proveedor y modelo de IA (sin credenciales aquí).</summary>
public sealed class AISettings
{
    public string Provider { get; set; } = "Ollama";

    public string Model { get; set; } = "llama3";
}

/// <summary>Dispositivos de audio preferidos.</summary>
public sealed class VoiceSettings
{
    public string Microphone { get; set; } = "Predeterminado";

    public string Speaker { get; set; } = "Predeterminado";

    public bool NoiseCancellation { get; set; } = true;
}

/// <summary>Apariencia de la interfaz.</summary>
public sealed class AppearanceSettings
{
    public ThemePreference Theme { get; set; } = ThemePreference.System;
}

/// <summary>Rutas favoritas de trabajo y herramientas.</summary>
public sealed class PathsSettings
{
    public string Workspace { get; set; } = string.Empty;

    public string VSCode { get; set; } = "code";

    public string ClaudeCode { get; set; } = "claude";
}

public sealed class FavoriteRouteSettings
{
    public string Alias { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
}

public sealed class FavoriteProjectSettings
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
}
