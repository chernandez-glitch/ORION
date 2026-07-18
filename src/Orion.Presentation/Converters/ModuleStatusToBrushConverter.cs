using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Orion.Shared.Modules;

namespace Orion.Presentation.Converters;

/// <summary>Traduce un <see cref="ModuleStatus"/> a un color de indicador.</summary>
public sealed class ModuleStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var color = value is ModuleStatus status
            ? status switch
            {
                ModuleStatus.Ready => Colors.SeaGreen,
                ModuleStatus.Degraded => Colors.Goldenrod,
                ModuleStatus.Offline => Colors.IndianRed,
                _ => Colors.Gray
            }
            : Colors.Gray;

        return new SolidColorBrush(color);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
