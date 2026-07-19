namespace Orion.Application.Commands.Abstractions;

/// <summary>
/// Puerto para mostrar diálogos al usuario desde un comando, sin acoplar la capa
/// de aplicación a WinUI. La implementación (ContentDialog) vive en Presentation.
/// </summary>
public interface IDialogService
{
    Task ShowMessageAsync(string title, string message);
}
