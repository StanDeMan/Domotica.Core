using Domotica.Core.Config;
using Newtonsoft.Json.Linq;

namespace Domotica.Core.Hardware
{
    /// <summary>
    /// Device state handling
    /// </summary>
    public static class Device
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!
        public static string Status { get; set; }

        public static bool IsConfigured => !string.IsNullOrEmpty(Status);

        public static string ReadNameFromConfig(string status)
        {
            return string.IsNullOrEmpty(status) 
                ? string.Empty 
                : SetName(status);
        }

        /// <summary>
        /// Read device name from appsettings.json config file
        /// </summary>
        /// <param name="status">Device json</param>
        /// <returns>Device json with set device name</returns>
        private static string SetName(string status)
        {
            dynamic data = JObject.Parse(status);
            data.Name = DeviceConfig.Name;
            Status = data.ToString();

            return Status;
        }
    }
}
