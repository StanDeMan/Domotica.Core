using Microsoft.AspNetCore.SignalR;

namespace Domotica.Core.Hubs
{
    public class Hello : Hub
    {
        public void Echo(string str)
        {
            Clients.Others.SendAsync("echo", str);
        }
    }
}

