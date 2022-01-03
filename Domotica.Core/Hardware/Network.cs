using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace Domotica.Core.Hardware
{
    // Internal Namespace: Domotica.Core
    using Models;
    using Execute;

    public class Network
    {
        public async Task<(bool, List<Parameter>)> ReadWifiNetworksAsync()
        {
            // Initialize empty list Wifi.Networks list
            // If running on windows: fill dummy list for tests
            var presentWifi = Platform.OperatingSystem == Platform.EnmOperatingSystem.Windows 
                ? DummyWifiForWindows() 
                : new Wifi();

            try
            {
                var retBash = await new Bash().ExecuteAsync(@"sudo iwlist wlan0 scan | grep 'ESSID:\|Quality\|Encryption'");

                var listWifi = retBash
                    .Split("Quality")
                    .Select(t => t.Trim())
                    .Select(s => s.Replace("\"",""))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList();

                foreach (var wifiInfo in listWifi.Select(wifi => Regex
                                 .Replace(wifi, @"\s+", ",")
                                 .Replace("=", "")
                                 .Replace("key:", "")
                                 .Replace("ESSID:", ""))
                             .Select(wifiInfoRow => wifiInfoRow.Split(",")))
                {
                    // calculate wifi quality
                    var fraction = wifiInfo[0].Split(@"/");
                    var quality = Convert.ToInt32(Convert.ToDouble(fraction[0]) / Convert.ToDouble(fraction[1]) * 100);

                    // add parameter to list
                    presentWifi.NetWorks.Add(new Parameter(wifiInfo[6], wifiInfo[5], quality));
                }
            }
            catch (Exception e)
            { 
                Log.Error($"Network.ReadWifiOptions exception: {e}");
                return (false, presentWifi.NetWorks);   // empty list
            }

            return (true, presentWifi.NetWorks);
        }

        /// <summary>
        /// Dummy wifi network list
        /// </summary>
        /// <returns>Dummy list</returns>
        private static Wifi DummyWifiForWindows()
        {
            var presentWifi = new Wifi();

            for (var i = 0; i < 4; i++)
            {
                presentWifi.NetWorks.Add(
                    new Parameter(
                        $"WifiNetwork{i}", 
                        i % 2 == 0 ? "on" : "off", 
                        new Random().Next(50, 100)));
            }

            return presentWifi;
        }

        public Mac Mac { get; set; } = new Mac();
    }

    public class Mac
    {
        /// <summary>
        /// Read first MAC address
        /// </summary>
        /// <returns></returns>
        public string ReadAddress()
        {
            return ReadNetWorks()
                .First()
                .GetPhysicalAddress()
                .ToString();
        }

        /// <summary>
        /// Read all present non wifi networks
        /// </summary>
        /// <returns>List of networks</returns>
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
