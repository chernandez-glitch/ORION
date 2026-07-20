using Orion.Shared.Results;

namespace Orion.Windows.Abstractions;

/// <summary>Operaciones de archivos y carpetas (con guardas de seguridad al borrar).</summary>
public interface IFileSystemService
{
    Result CreateFolder(string path);

    Result DeleteFolder(string path, bool recursive = true);

    Result MoveFolder(string source, string destination);

    Result CopyFolder(string source, string destination);

    Result CreateFile(string path, string? content = null);

    Result DeleteFile(string path);

    Result CopyFile(string source, string destination, bool overwrite = true);

    Result MoveFile(string source, string destination);

    Result Rename(string path, string newName);

    Result Zip(string sourceDirectory, string zipPath);

    Result Unzip(string zipPath, string destinationDirectory);
}
