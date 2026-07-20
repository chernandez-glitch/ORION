using Orion.Shared.Results;

namespace Orion.Windows.Abstractions;

/// <summary>Lee y escribe el portapapeles.</summary>
public interface IClipboardService
{
    Result<string> GetText();

    Result SetText(string text);

    /// <summary>Pega en la ventana enfocada (envía Ctrl+V).</summary>
    Result Paste();

    Result<IReadOnlyList<string>> GetFiles();

    Result SetFiles(IEnumerable<string> paths);
}
