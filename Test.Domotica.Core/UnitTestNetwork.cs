using System.Diagnostics;
using Domotica.Core.Hardware;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domotica.Core
{
    [TestClass]
    public sealed class UnitTestNetwork
    {
        [TestMethod]
        public void TestReadMacAddress()
        {
            var mac = new Mac();
            var macAddress = mac.ReadAddress();

            Assert.IsNotNull(macAddress);
            Debug.Print($"Read MAC address: {macAddress}");
        }

        [TestMethod]
        public void TestReadMacAddresses()
        {
            var mac = new Mac();
            var macAddresses = mac.ReadAddresses();

            Assert.AreNotEqual(0, macAddresses.Count);
           
            Debug.Print($"MAC addresses found: {macAddresses.Count}");

            foreach (var macAddress in macAddresses)
            {
                Debug.Print($"MAC address: {macAddress.GetPhysicalAddress()}");
            }
        }

        [TestMethod]
        public void TestReadMacAddressesViaNetwork()
        {
            var network = new Network();
            var macAddresses = network.Mac.ReadAddresses();

            Assert.AreNotEqual(0, macAddresses.Count);
           
            Debug.Print($"MAC addresses found: {macAddresses.Count}");

            foreach (var macAddress in macAddresses)
            {
                Debug.Print($"MAC address: {macAddress.GetPhysicalAddress()}");
            }
        }

        [TestMethod]
        public void TestReadWifiNetworkOptions()
        {
            var (ok, wifiNetworks) = new Network().ReadWifiNetworksAsync().Result;
            Assert.IsFalse(ok);
            Assert.IsNotNull(wifiNetworks);

            foreach (var wifiNetwork in wifiNetworks)
            {
                Debug.Print($"SSID: {wifiNetwork.SsId}, Encryption: {wifiNetwork.Encryption}, Quality: {wifiNetwork.Quality}");
            }
        }
    }
}