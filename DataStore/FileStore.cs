using Serilog;
using Newtonsoft.Json.Linq;
using JsonFlatFileDataStore;

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

        public async Task<bool> InsertAsync(string json)
        {
            try
            {
                var device = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?.InsertOneAsync(device)!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.InsertAsync: {e}");
                
                return false;
            }
        }

        public async Task<dynamic?> ReadAsync(string json)
        {
            try
            {
                var device = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);
                
                return await Task.Run(() => collection?
                    .AsQueryable()
                    .FirstOrDefault(d => d.DeviceId == device.Value<string>("DeviceId")));
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.ReadAsync: {e}");

                return null;
            }
        }

        public async Task<bool> UpdateAsync(string json)
        {
            try
            {
                var device = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?
                    .UpdateOneAsync(d => d.DeviceId == device.Value<string>("DeviceId"), device)!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.UpdateAsync: {e}");
                
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string json)
        {
            try
            {
                var device = JToken.Parse(json);
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?
                    .DeleteOneAsync(d => d.DeviceId == device.Value<string>("DeviceId"))!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.DeleteAsync: {e}");
                
                return false;
            }
        }

        public void Dispose()
        {
            DataStore?.Dispose();
        }
    }
}