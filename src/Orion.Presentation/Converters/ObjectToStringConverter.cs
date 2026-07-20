using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace Orion.Presentation.Converters;

/// <summary>Convierte cualquier valor (fecha, número, enum) a texto para bindings a TextBlock.</summary>
public sealed class ObjectToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value switch
    {
        null => string.Empty,
        DateTime date => date.ToString("g", CultureInfo.CurrentCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
        _ => value.ToString() ?? string.Empty
    };

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
