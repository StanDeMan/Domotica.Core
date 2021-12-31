using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Domotica.Core.Hardware
{
    public class Network
    {
        public void ReadEntries()
        {
            throw new NotImplementedException("Network.Read not implemented!");
        }

        public Mac Mac { get; set; } = new Mac();
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

        protected static IEnumerable<NetworkInterface> ReadNetWorks()
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
