using System.Threading.Tasks;
using Domotica.Core.Hardware;
using Microsoft.AspNetCore.SignalR;

namespace Domotica.Core.Hubs
{
    public class Device : Hub
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!
        private static string deviceStatus;

        public void SendCommand(string value)
        {
            Command.Execute(value);
        }

        public async Task SendDeviceStatus(string value)
        {
            deviceStatus = value;
            await Clients.Others.SendAsync("deviceStatusReceived", value);
        }

        public async Task GetDeviceStatusInitial()
        {
            await Clients.Caller.SendAsync("deviceStatusInitial", deviceStatus);
        }
    }
}
