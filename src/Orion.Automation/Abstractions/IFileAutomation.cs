using Orion.Shared.Results;

namespace Orion.Automation.Abstractions;

/// <summary>Puerto para operaciones de archivos y carpetas.</summary>
public interface IFileAutomation
{
    Task<Result> CopyAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken = default);

    Task<Result> MoveAsync(string source, string destination, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(string path, CancellationToken cancellationToken = default);
}
