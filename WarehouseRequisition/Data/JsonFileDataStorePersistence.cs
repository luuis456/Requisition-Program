using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using WarehouseRequisition.Configuration;

namespace WarehouseRequisition.Data;

/// <summary>
/// Persists the in-memory store to a local JSON file so restarting the prototype does not lose data.
/// This is the only piece to replace when moving to EF Core + PostgreSQL.
/// </summary>
public class JsonFileDataStorePersistence : IDataStorePersistence
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _filePath;

    public JsonFileDataStorePersistence(IOptions<StorageOptions> options, IHostEnvironment environment)
    {
        _filePath = Path.IsPathRooted(options.Value.FilePath)
            ? options.Value.FilePath
            : Path.Combine(environment.ContentRootPath, options.Value.FilePath);
    }

    public string FilePath => _filePath;

    public bool Exists => File.Exists(_filePath);

    public void Load(InMemoryDataStore store)
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var loaded = JsonSerializer.Deserialize<InMemoryDataStore>(json, SerializerOptions);
            if (loaded is null)
            {
                return;
            }

            store.Users = loaded.Users;
            store.Plants = loaded.Plants;
            store.Areas = loaded.Areas;
            store.Machines = loaded.Machines;
            store.Parts = loaded.Parts;
            store.PartLocations = loaded.PartLocations;
            store.ShortageReasons = loaded.ShortageReasons;
            store.Requisitions = loaded.Requisitions;
        }
        catch (JsonException)
        {
            // A corrupted snapshot falls back to seed data instead of crashing the prototype.
        }
    }

    public void Save(InMemoryDataStore store)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tempPath = $"{_filePath}.tmp";
        var json = JsonSerializer.Serialize(store, SerializerOptions);
        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }
}
