using Microsoft.UI.Xaml.Media;

namespace Orion.Presentation.Models;

/// <summary>
/// Ítem de estado para la top bar con su color ya resuelto. Se precalcula el
/// pincel para evitar convertidores en el DataTemplate (no soportados en x:Bind
/// dentro de una Window).
/// </summary>
public sealed class StatusPillItem(string module, string detail, Brush statusBrush)
{
    public string Module { get; } = module;

    public string Detail { get; } = detail;

    public Brush StatusBrush { get; } = statusBrush;
}
