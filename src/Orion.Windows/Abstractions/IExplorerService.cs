using Orion.Shared.Results;

namespace Orion.Windows.Abstractions;

/// <summary>Interactúa con el Explorador de Windows.</summary>
public interface IExplorerService
{
    Result OpenFolder(string path);

    Result OpenFolders(IEnumerable<string> paths);

    /// <summary>Abre el Explorador y selecciona el archivo indicado.</summary>
    Result SelectFile(string filePath);

    Result OpenLocation(string path);

    /// <summary>Muestra el diálogo de propiedades de un archivo o carpeta.</summary>
    Result ShowProperties(string path);
}
