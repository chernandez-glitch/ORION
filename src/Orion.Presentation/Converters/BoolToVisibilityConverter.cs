using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Orion.Presentation.Converters;

/// <summary>
/// Convierte un booleano a <see cref="Visibility"/>. El parámetro "invert"
/// invierte el resultado.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is true;
        if (string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase))
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
