using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Exporta e importa la memoria (JSON completo, CSV por tabla).</summary>
public interface IMemoryExportService
{
    Task<Result<string>> ExportJsonAsync(string filePath, CancellationToken cancellationToken = default);

    Task<Result<string>> ExportCsvAsync(string directoryPath, CancellationToken cancellationToken = default);

    Task<Result> ImportJsonAsync(string filePath, CancellationToken cancellationToken = default);
}
