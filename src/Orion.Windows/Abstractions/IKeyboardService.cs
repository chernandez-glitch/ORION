using Orion.Shared.Results;

namespace Orion.Windows.Abstractions;

/// <summary>Simula entrada de teclado (SendInput).</summary>
public interface IKeyboardService
{
    /// <summary>Escribe texto como pulsaciones de teclado.</summary>
    Result SendText(string text);

    /// <summary>Envía una tecla individual: "Enter", "Esc", "Tab", "Delete", "F5"…</summary>
    Result SendKey(string key);

    /// <summary>Envía una combinación: "Ctrl+C", "Ctrl+Shift+Esc", "Win+D"…</summary>
    Result SendCombination(string combination);
}
