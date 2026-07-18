using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Orion.Configuration.Models;
using Orion.Shared.Results;

namespace Orion.Configuration;

/// <summary>
/// Implementación de <see cref="IConfigurationService"/> respaldada por un
/// archivo JSON en el perfil del usuario. La escritura es atómica (archivo
/// temporal + reemplazo) para no corromper la configuración ante un fallo.
/// </summary>
public sealed class JsonConfigurationService : IConfigurationService, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private readonly ILogger<JsonConfigurationService> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonConfigurationService(string filePath, ILogger<JsonConfigurationService> logger)
    {
        _filePath = filePath;
        _logger = logger;
        Current = new OrionSettings();
    }

    public OrionSettings Current { get; private set; }

    public event EventHandler<OrionSettings>? Changed;

    public async Task<Result<OrionSettings>> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogInformation("No hay configuración previa en {Path}; se usan valores por defecto.", _filePath);
                Current = new OrionSettings();
                return Current;
            }

            await using var stream = File.OpenRead(_filePath);
            var settings = await JsonSerializer.DeserializeAsync<OrionSettings>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            Current = settings ?? new OrionSettings();
            return Current;
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Error al leer la configuración desde {Path}.", _filePath);
            return Result.Failure<OrionSettings>(ConfigurationErrors.ReadFailed(ex.Message));
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Result> SaveAsync(OrionSettings settings, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempPath = _filePath + ".tmp";
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(stream, settings, SerializerOptions, cancellationToken).ConfigureAwait(false);
            }

            File.Move(tempPath, _filePath, overwrite: true);
            Current = settings;
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Error al guardar la configuración en {Path}.", _filePath);
            return Result.Failure(ConfigurationErrors.WriteFailed(ex.Message));
        }
        finally
        {
            _gate.Release();
        }

        Changed?.Invoke(this, settings);
        return Result.Success();
    }

    public void Dispose() => _gate.Dispose();
}
