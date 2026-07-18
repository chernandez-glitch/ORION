namespace Orion.Automation.Models;

/// <summary>Información mínima de un proceso del sistema.</summary>
/// <param name="Id">PID del proceso.</param>
/// <param name="Name">Nombre del ejecutable sin extensión.</param>
/// <param name="MainWindowTitle">Título de la ventana principal, si tiene.</param>
public sealed record ProcessInfo(int Id, string Name, string MainWindowTitle);
