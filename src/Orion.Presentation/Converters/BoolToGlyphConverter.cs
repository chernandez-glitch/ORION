using Microsoft.UI.Xaml.Data;

namespace Orion.Presentation.Converters;

/// <summary>Verdadero → check; falso → cruz (glifos de Segoe Fluent Icons).</summary>
public sealed class BoolToGlyphConverter : IValueConverter
{
    // Code points de Segoe Fluent Icons: E73E = CheckMark, E711 = Cancel.
    private static readonly string CheckGlyph = char.ConvertFromUtf32(0xE73E);
    private static readonly string CancelGlyph = char.ConvertFromUtf32(0xE711);

    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is true ? CheckGlyph : CancelGlyph;

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
