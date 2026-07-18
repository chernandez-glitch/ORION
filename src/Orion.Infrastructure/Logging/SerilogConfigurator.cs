using System.Globalization;
using Serilog;
using Serilog.Events;

namespace Orion.Infrastructure.Logging;

/// <summary>
/// Configura Serilog para ORION: consola (para desarrollo) y archivo rotativo
/// diario en el directorio de logs del usuario.
/// </summary>
public static class SerilogConfigurator
{
    public static Serilog.Core.Logger Create(string logDirectory)
    {
        Directory.CreateDirectory(logDirectory);
        var logFile = Path.Combine(logDirectory, "orion-.log");

        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File(
                logFile,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                formatProvider: CultureInfo.InvariantCulture,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static string DefaultLogDirectory()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "OrionAI", "logs");
    }
}
