using CommunityToolkit.Mvvm.ComponentModel;

namespace Orion.Presentation.ViewModels;

/// <summary>Información del producto.</summary>
public sealed partial class AboutViewModel : ObservableObject
{
    public string ProductName { get; } = "ORION AI";

    public string Version { get; } = "0.1.0 — Fase 0/2";

    public string Tagline { get; } = "Asistente inteligente para Windows inspirado en Jarvis.";

    public string Description { get; } =
        "Plataforma modular de IA para controlar Windows, herramientas de desarrollo e " +
        "integraciones empresariales. Construida con .NET 10, WinUI 3 y Clean Architecture.";

    public string Stack { get; } = ".NET 10 · C# · WinUI 3 · SQLite · Serilog · MVVM";

    public string Company { get; } = "Grupo Platino";
}
