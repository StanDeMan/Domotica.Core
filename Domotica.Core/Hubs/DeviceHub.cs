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

        public async Task DeviceStatusSend(string device, string group)
        {
            Devices.AddOrUpdate(group, device);
            
            await Clients.OthersInGroup(group).SendAsync("deviceStatusReceived", device);
        }

        public async Task GetDeviceStatusInitial(string device, string group)
        {
            Devices.ChangeNameFromConfig(device);
            var (ok, storedDevice) = Devices.Read(group);
            
            // check if device is stored
            switch (ok)
            {
                // not stored: add a new one
                case false when !string.IsNullOrEmpty(storedDevice):
                    Devices.AddOrUpdate(@group, device);
                    break;

                default:
                    // take the stored one
                    device = storedDevice;
                    break;
            }

            await JoinGroup(group);
            await Clients.Caller.SendAsync("deviceStatusInitial", device);
        }
        
        public async Task SetDeviceStatusFinal(string group)
        {
            await LeaveGroup(group);
        }

        private async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        private async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
