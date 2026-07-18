using Orion.Shared.Results;

namespace Orion.Configuration;

public static class ConfigurationErrors
{
    public static Error ReadFailed(string detail) =>
        Error.Failure("Configuration.ReadFailed", $"No se pudo leer la configuración: {detail}");

    public static Error WriteFailed(string detail) =>
        Error.Failure("Configuration.WriteFailed", $"No se pudo guardar la configuración: {detail}");
}
