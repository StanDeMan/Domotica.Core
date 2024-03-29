﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBase;
using Domotica.Core.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

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
        private static readonly FileStore Store = new();

        public static string ChangeNameFromConfig(string device)
        {
            return string.IsNullOrEmpty(device) 
                ? string.Empty 
                : SetName(device);
        }

        public static async Task<bool> AddOrUpdate(string key, string device)
        {
            try
            {
                // update existing
                if(DeviceList.TryGetValue(key, out _))
                {
                    DeviceList[key] = device;
                    await Store.UpdateAsync(device);

                    return true;
                }
                
                // add new
                DeviceList.Add(key, device);
                await Store.InsertAsync(device);

                return true;

            }
            catch (Exception e)
            {
                Log.Error($"Devices.AddOrUpdate: {e}");

                return false;
            }
        }

        public static bool Delete(string key)
        {
            return DeviceList.Remove(key);
        }

        public static async Task<(bool, string)> Read(string key)
        {
            try
            {
                // check if present in list
                if(DeviceList.TryGetValue(key, out var value))
                    return (true, value);
                
                // check if present in database
                var device = await Store.ReadByNameIdAsync(key);

                return ((bool, string))(device?.NameId == key 
                    ? (true, JsonConvert.SerializeObject(device)) 
                    : (false, string.Empty));
            }
            catch (Exception e)
            {
                Log.Error($"Devices.Read: {e}");

                return (false, string.Empty);
            }
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
