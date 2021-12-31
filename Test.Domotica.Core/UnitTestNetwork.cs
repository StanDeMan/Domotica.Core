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
    }
}