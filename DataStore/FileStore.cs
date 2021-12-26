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

        /// <summary>
        /// Flat file data store
        /// </summary>
        /// <param name="dataBaseName">Name of file to create</param>
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

        /// <summary>
        /// Insert json 
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true if inserted</returns>
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

        /// <summary>
        /// Read json
        /// </summary>
        /// <param name="json">Read this entity</param>
        /// <returns>Json as dynamic</returns>
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

        /// <summary>
        /// Read json
        /// </summary>
        /// <param name="id">Search for this id</param>
        /// <returns>Json as dynamic</returns>
        public async Task<dynamic?> ReadByIdAsync(int id)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);
                
                return await Task.Run(() => collection?
                    .AsQueryable()
                    .FirstOrDefault(d => d.Id == id));
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.ReadAsync: {e}");

                return null;
            }
        }

        /// <summary>
        /// Read json
        /// </summary>
        /// <param name="deviceId">Search for deviceId</param>
        /// <returns>Json as dynamic</returns>
        public async Task<dynamic?> ReadByDeviceIdAsync(string deviceId)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);
                
                return await Task.Run(() => collection?
                    .AsQueryable()
                    .FirstOrDefault(d => d.DeviceId == deviceId));
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.ReadAsync: {e}");

                return null;
            }
        }

        /// <summary>
        /// Update json
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true id updated</returns>
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

        /// <summary>
        /// Delete json
        /// </summary>
        /// <param name="json">This entity</param>
        /// <returns>true if deleted</returns>
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

        /// <summary>
        /// Delete json
        /// </summary>
        /// <param name="deviceId">Search for deviceId</param>
        /// <returns>true if deleted</returns>
        public async Task<bool> DeleteByDeviceIdAsync(string deviceId)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?
                    .DeleteOneAsync(d => d.DeviceId == deviceId)!;
            }
            catch (Exception e)
            {
                Log.Error($"DataBase.File.DeleteAsync: {e}");
                
                return false;
            }
        }

        /// <summary>
        /// Delete json
        /// </summary>
        /// <param name="id">Search for this id</param>
        /// <returns>true if deleted</returns>
        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                var collection = DataStore?.GetCollection(CollectionName);

                return await collection?
                    .DeleteOneAsync(d => d.Id == id)!;
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