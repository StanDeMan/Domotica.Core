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
            var network = new Mac();
            var mac = network.ReadAddress();

            Assert.IsNotNull(mac);
            Debug.Print($"Read MAC address: {mac}");
        }

        [TestMethod]
        public void TestReadMacAddresses()
        {
            var network = new Mac();
            var networks = network.ReadAddresses();

            Assert.AreNotEqual(0, networks.Count);
           
            Debug.Print($"MAC addresses found: {networks.Count}");

            foreach (var net in networks)
            {
                Debug.Print($"MAC address: {net.GetPhysicalAddress()}");
            }
        }
    }
}