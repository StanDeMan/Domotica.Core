using System;
using System.Collections.Generic;
using Domotica.Core.Config;
using Newtonsoft.Json.Linq;

namespace Domotica.Core.Hardware
{
    /// <summary>
    /// Device state handling
    /// </summary>
    public static class Devices
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!
        private static readonly Dictionary<string, string> DeviceList = new();

        public static string ChangeNameFromConfig(string device)
        {
            return string.IsNullOrEmpty(device) 
                ? string.Empty 
                : SetName(device);
        }

        public static bool AddOrUpdate(string key, string newValue)
        {
            try
            {
                if(DeviceList.TryGetValue(key, out _)) DeviceList[key] = newValue;
                else DeviceList.Add(key, newValue);
            }
            catch (Exception)
            {
                //TODO: do logging here...
                return false;
            }

            return true;
        }

        public static bool Delete(string key)
        {
            return DeviceList.Remove(key);
        }

        public static (bool, string) Read(string key)
        {
            var ok = DeviceList.TryGetValue(key, out var value);

            return (ok, value);
        }

        public static int Count()
        {
            return DeviceList.Count;
        }

        /// <summary>
        /// Read device name from appsettings.json config file
        /// </summary>
        /// <param name="status">Device json</param>
        /// <returns>Device json with set device name</returns>
        private static string SetName(string status)
        {
            dynamic data = JObject.Parse(status);
            
            // if other name set -> use it
            if (DeviceConfig.Name != string.Empty)
            {
                data.Name = DeviceConfig.Name;
            }

            return data.Name;
        }
    }
}
