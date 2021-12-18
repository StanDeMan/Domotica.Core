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

        private string SetName(string status)
        {
            dynamic data = JObject.Parse(status);
            data.Name = "Main Light";
            Status = data.ToString();

            return Status;
        }
    }
}
