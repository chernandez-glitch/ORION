namespace Orion.Application.Abstractions;

/// <summary>Reloj respaldado por el reloj del sistema.</summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
