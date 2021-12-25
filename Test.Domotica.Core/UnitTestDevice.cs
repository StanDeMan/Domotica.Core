using System.Diagnostics;
using Domotica.Core.Hardware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Device = Hardware.Device;


namespace Test.Domotica.Core
{
    [TestClass]
    public class UnitTestDevice
    {
        [TestMethod]
        public void TestDevice()
        {
            using var apa102 = new Device(8);

            switch (Platform.OperatingSystem)
            {
                case Platform.EnmOperatingSystem.Linux:
                {
                    var ready = apa102.IsReady;
                    Assert.AreEqual(true, ready);
                    Debug.Print($"Operating System: {Platform.EnmOperatingSystem.Linux}");

                    apa102.Switch(ready 
                        ? Device.EnmState.On 
                        : Device.EnmState.Off);

                    apa102.Switch(Device.EnmState.Off);
                    break;
                }
                case Platform.EnmOperatingSystem.Windows:
                {
                    var notReady = apa102.IsReady;

                    Assert.AreEqual(false, notReady);
                    Debug.Print($"Operating System: {Platform.EnmOperatingSystem.Windows}");
                    break;
                }
                case Platform.EnmOperatingSystem.Unknown:
                case Platform.EnmOperatingSystem.Mac:
                default:
                    Debug.Print($"Not supported so far: {Platform.OperatingSystem}");
                    break;
            }
        }
    }
}
