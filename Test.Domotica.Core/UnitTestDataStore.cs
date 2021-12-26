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
                    'Name': 'AmbientLigth',
                    'LedRGBStripe': {
                        'Brightness': 0,
                        'LastValue': 100,
                        'Color': {
                            'R': 0,
                            'G': 0,
                            'B': 0
                        }
                    }
                }";


            var device = JToken.Parse(json);
            var deviceId = device.Value<string>("DeviceId") ?? "";

            var ok = await store.InsertAsync(json);
            Assert.IsTrue(ok);

            var (okRead, readJson) = await store.ReadAsync(deviceId);
            Assert.IsTrue(okRead);

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

            (okRead, readJson) = await store.ReadAsync(deviceId);
            Assert.IsTrue(okRead);
            Assert.AreEqual("LedStripe", readJson.Name);

            ok = await store.DeleteAsync(deviceId);
            Assert.IsTrue(ok);

            File.Delete($"{store.DataBaseName}.json");
        }
    }
}
