using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Domotica.Core.Hardware
{
    public class Network
    {
        public Mac Mac { get; } = new Mac();
    }

    public class Mac
    {
        public string ReadAddress()
        {
            return ReadNetWorks()
                .First()
                .GetPhysicalAddress()
                .ToString();
        }

        public ICollection<NetworkInterface> ReadAddresses()
        {
            return ReadNetWorks().ToList();
        }

        private IEnumerable<NetworkInterface> ReadNetWorks()
        {
            var activeNetworks = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(ni => 
                    ni.OperationalStatus == OperationalStatus.Up && 
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);   
            
            return activeNetworks.OrderByDescending(ni => ni.Speed);
        }
    }
}
