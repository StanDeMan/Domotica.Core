using System.IO;
using Domotica.Core.Functionality;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test.Domotica.Core
{
    [TestClass]
    public sealed class UnitTestFunctionality
    {
        public static string? TestDir { get; set; }

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            TestDir = Path.GetDirectoryName(Path.GetDirectoryName(testContext.TestDeploymentDir));
        }

        [TestMethod]
        public void TestDeviceAssembly()
        {
            const string jsonCmdParams =
                @"{
                    'Type': 'Apa102',
                    'External': {
                        'Assembly': 'Hardware',
                        'ClassName': 'Device',
                        'Method': 'Dimmer'
                    },
                    'LedAmount': 8,
                    'Color': {
                        'A': 1,
                        'R': 0,
                        'G': 255,
                        'B': 255
                    }
                }";

            if (string.IsNullOrEmpty(TestDir)) Assert.Fail($"Test execution path not found: {TestDir}");
            var execDir = $@"{TestDir}\Debug\net6.0" ;

            var assembly = new ExtendAssembly(execDir, "Hardware", "Device");
            if(!assembly.IsLoaded) Assert.Fail("Assembly not loaded.");

            dynamic ambientLigth = JsonConvert.DeserializeObject(jsonCmdParams)!;

            var param = new object[1];
            param[0] = ambientLigth;            

            var type = new[] { typeof(object) };

            assembly.Method?.Execute("Dimmer", type, param);
        }
    }
}
