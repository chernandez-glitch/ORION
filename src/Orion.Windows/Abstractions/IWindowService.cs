using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Controla ventanas de nivel superior (por coincidencia parcial de título).</summary>
public interface IWindowService
{
    Result<IReadOnlyList<WindowDetails>> GetOpenWindows();

    Result<WindowDetails> Find(string title);

    Result BringToFront(string title);

    Result Minimize(string title);

    Result Maximize(string title);

    Result Close(string title);

    Result Hide(string title);

    Result Show(string title);

    Result Resize(string title, int width, int height);

    Result Move(string title, int x, int y);
}
