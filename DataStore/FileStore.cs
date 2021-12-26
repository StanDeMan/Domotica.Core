using Newtonsoft.Json.Linq;
using JsonFlatFileDataStore;
using Serilog;

namespace DataBase
{
    public class FileStore : IDisposable
    {
        private const string DbName = "database";
        private const string CollectionName = "devices";

        public bool IsRunning { get; set; }
       
        public string? DataBaseName { get; } = DbName;

        public DataStore? DataStore { get; }

        public FileStore(string? dataBaseName = null)
        {
            try
            {
                // Generate store with upper camel case json
                DataStore = dataBaseName == null 
                    ? new DataStore($"{DataBaseName}.json", false) 
                    : new DataStore($"{DbName}.json", false);

                DataBaseName = dataBaseName ?? DataBaseName;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Warning()
                    .WriteTo.File(@$"logs\{DataBaseName}.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                IsRunning = true;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File initializing: {e}");

                IsRunning = false;
            }
        }

        public async Task<(bool, int)> InsertAsync(string json)
        {
            try
            {
                var deviceJson = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);
                var ok = await collection?.InsertOneAsync(deviceJson)!;
                int nextId = Convert.ChangeType(collection.GetNextIdValue(), typeof(int));
                
                return (ok, --nextId);
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.Insert: {e}");
                
                return (false, 0);
            }
        }

        public async Task<(bool, dynamic)> ReadAsync(int id)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);

                var result =  await Task.Run(() => collection?
                    .AsQueryable()
                    .FirstOrDefault(e => e.Id == id)!);

                return (true, result);
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.Read: {e}");

                return (false, string.Empty);
            }
        }

        public async Task<bool> UpdateAsync(int id, string json)
        {
            try
            {
                var deviceJson = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?.UpdateOneAsync(e => e.Id == id, deviceJson)!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.Update: {e}");
                
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);
                
                return await collection?.DeleteOneAsync(e => e.Id == id)!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.Delete: {e}");
                
                return false;
            }
        }

        public void Dispose()
        {
            DataStore?.Dispose();
        }
    }
}