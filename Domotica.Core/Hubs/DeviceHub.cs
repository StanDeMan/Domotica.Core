using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Domotica.Core.Hardware;
using Microsoft.AspNetCore.SignalR;

namespace Domotica.Core.Hubs
{
    public class Device : Hub
    {
        // Container for device status implemented on the device html page.
        // Every device knows what it is and how to deal with related data!
        private readonly Hardware.Device _hardWareDevice = new Hardware.Device();

        public void SendCommand(string value)
        {
            Command.Execute(value);
        }

        public async Task DeviceStatusSend(string device)
        {
            _hardWareDevice.Status = device;            
            await Clients.Others.SendAsync("deviceStatusReceived", device);
        }

        public async Task GetDeviceStatusInitial(string device)
        {
            _hardWareDevice.ReadName(device);
            
            await Clients.Caller.SendAsync("deviceStatusInitial", _hardWareDevice.Status);
        }
    }
}
