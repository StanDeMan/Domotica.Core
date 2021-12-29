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

        public async Task DeviceStatusSend(string device, string group)
        {
            Hardware.Device.Status = device;            
            await Clients.OthersInGroup(group).SendAsync("deviceStatusReceived", device);
        }

        public async Task GetDeviceStatusInitial(string device, string group)
        {
            if (!Hardware.Device.IsConfigured)
            {
                Hardware.Device.ReadNameFromConfig(device);
            }

            await JoinGroup(group);
            await Clients.Caller.SendAsync("deviceStatusInitial", Hardware.Device.Status);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
