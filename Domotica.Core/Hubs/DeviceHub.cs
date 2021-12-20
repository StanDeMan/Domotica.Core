using System.Threading.Tasks;
using Domotica.Core.Hardware;
using Microsoft.AspNetCore.SignalR;

namespace Domotica.Core.Hubs
{
    public class Device : Hub
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!

        public void SendCommand(string value)
        {
            Command.Execute(value);
        }

        public void SendAmbientCommand(string value)
        {
            Command.ExecuteAmbient(value);
        }

        public async Task DeviceStatusSend(string device)
        {
            Hardware.Device.Status = device;            
            await Clients.Others.SendAsync("deviceStatusReceived", device);
        }

        public async Task GetDeviceStatusInitial(string device)
        {
            if (!Hardware.Device.IsConfigured)
            {
                Hardware.Device.ReadNameFromConfig(device);
            }

            await Clients.Caller.SendAsync("deviceStatusInitial", Hardware.Device.Status);
        }
    }
}
