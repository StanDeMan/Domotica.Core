using System.IO;
using DataBase;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test.Domotica.Core
{
    [TestClass]
    public sealed class UnitTestDataStore
    {
        [TestInitialize]
        public void TestInitialize()
        {
            File.Delete("database.json");
        }   

        [TestMethod]
        public async Task TestDataStore()
        {
            using var store = new FileStore();

            if (!store.IsRunning)
            {
                Assert.Fail("FileStore not running!");
            }

            const string json = 
                @"{
                    'DeviceId': '67DJzL3xwLCT', 
                    'Name': 'LedRGBStripe',
                    'Device': {
                        'LastValue': 100,
                        'Color': {
                            'A': 0.65,
                            'R': 0,
                            'G': 0,
                            'B': 0
                        }
                    },
                    'Id': 0
                }";

            const string jsonAmbient =
                @"{
                    'DeviceId': '76JDzL3xwLCT',
                    'Name': 'AmbientLigth',
                    'Device': {
                        'Color': {
                            'A': 0.85,
                            'R': 0,
                            'G': 255,
                            'B': 255
                        }
                    }
                }";

            var device = JToken.Parse(json);

            var ok = await store.InsertAsync(json);
            Assert.IsTrue(ok);

            var readJson = await store.ReadAsync(json);
            Assert.IsNotNull(readJson);

            var storedJson = JsonConvert.SerializeObject(readJson);
            var oldJson = JsonConvert.SerializeObject(device);

            var equal = JToken.DeepEquals(storedJson, oldJson);
            Assert.IsTrue(equal);
            
            // Update the value of the property: Name -> set to LedStripe
            var jObject = JsonConvert.DeserializeObject(json) as JObject;
            var jToken = jObject?.SelectToken("Name")!;
            jToken.Replace("LedStripe");

            ok = await store.UpdateAsync(jObject?.ToString()!);
            Assert.IsTrue(ok);

            readJson = await store.ReadAsync(json);
            Assert.IsNotNull(readJson);
            Assert.AreEqual("LedStripe", readJson?.Name);

            ok = await store.InsertAsync(jsonAmbient);
            Assert.IsTrue(ok);

            readJson = await store.ReadAsync(jsonAmbient);
            Assert.IsNotNull(readJson);
            Assert.AreEqual("76JDzL3xwLCT", readJson?.DeviceId);

            ok = await store.DeleteAsync(json);
            Assert.IsTrue(ok);

            ok = await store.DeleteAsync(jsonAmbient);
            Assert.IsTrue(ok);

            File.Delete($"{store.DataBaseName}.json");
        }
    }
}
