namespace Orion.Memory.Engine;

/// <summary>Mantiene el Id de la sesión activa (creada al arrancar ORION).</summary>
public interface IMemorySession
{
    Guid? CurrentSessionId { get; set; }
}

internal sealed class MemorySession : IMemorySession
{
    public Guid? CurrentSessionId { get; set; }
}
