using System;
using System.IO;
using Domotica.Core.Functionality;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Test.Domotica.Core
{
    [TestClass]
    public sealed class UnitTestImportAssembly
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
                        'Class': 'Device',
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

            dynamic cmdParams = JsonConvert.DeserializeObject(jsonCmdParams)!;

            var assemblyName = Convert.ToString(cmdParams.External.Assembly); 
            var className = Convert.ToString(cmdParams.External.Class);
            var methodName = Convert.ToString(cmdParams.External.Method);

            var assembly = new ImportAssembly(execDir, assemblyName, className);
            if(!assembly.IsLoaded) Assert.Fail("Assembly not loaded.");

            // object created from json: method execution parameters
            var param = new object[1];
            param[0] = cmdParams;            
            
            // type of parameter: dynamic -> so take object
            var type = new[] { typeof(object) };

            assembly.Method?.Execute(methodName, type, param);
        }
    }
}
