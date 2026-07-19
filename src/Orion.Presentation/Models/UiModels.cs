namespace Orion.Presentation.Models;

/// <summary>Evento del panel de actividad del dashboard.</summary>
public sealed record ActivityEvent(string Time, string Title, string Detail, int GlyphCode)
{
    public string Glyph => char.ConvertFromUtf32(GlyphCode);
}

/// <summary>Entrada de log simulada.</summary>
public sealed record LogEntry(string Time, string Level, string Message);

/// <summary>Conversación simulada (la IA real llega en Fase 2).</summary>
public sealed record ConversationItem(string Title, string Preview, string When);

/// <summary>Automatización simulada (la ejecución real llega en Fase 1).</summary>
public sealed record AutomationItem(string Name, string Trigger, bool Enabled);

/// <summary>Integración de plugin prevista.</summary>
public sealed record PluginItem(string Name, string Version, string Description, bool Loaded);

/// <summary>Tarjeta de métrica del dashboard.</summary>
public sealed record StatCard(string Title, string Value, string Caption, int GlyphCode)
{
    public string Glyph => char.ConvertFromUtf32(GlyphCode);
}
