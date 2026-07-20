using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Simula entrada de mouse (SendInput).</summary>
public interface IMouseService
{
    Result MoveTo(int x, int y);

    Result Click();

    Result RightClick();

    Result DoubleClick();

    Result Scroll(int amount);

    Result<MousePosition> GetPosition();
}
