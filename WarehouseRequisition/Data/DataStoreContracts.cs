namespace WarehouseRequisition.Data;

public interface IDataStorePersistence
{
    string FilePath { get; }

    bool Exists { get; }

    void Load(InMemoryDataStore store);

    void Save(InMemoryDataStore store);
}

public interface IDataStoreSeeder
{
    void SeedIfEmpty();

    void ResetToSeedData();
}
