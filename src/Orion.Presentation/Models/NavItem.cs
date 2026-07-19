namespace Orion.Presentation.Models;

/// <summary>
/// Ítem de navegación del sidebar. El glifo se expresa por su code point de
/// Segoe Fluent Icons para evitar caracteres literales en el código fuente.
/// </summary>
public sealed class NavItem(string key, string label, int glyphCode)
{
    public string Key { get; } = key;

    public string Label { get; } = label;

    public string Glyph { get; } = char.ConvertFromUtf32(glyphCode);
}
