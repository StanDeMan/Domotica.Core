using Domotica.Core.Config;
using Newtonsoft.Json.Linq;

namespace Domotica.Core.Hardware
{
    public class Device
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!
        public string Status { get; set; }

        public string ReadName(string status)
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
        private string SetName(string status)
        {
            dynamic data = JObject.Parse(status);
            data.Name = PropertyConfig.Name;
            Status = data.ToString();

            return Status;
        }
    }
}
