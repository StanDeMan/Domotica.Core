using System.IO;
using DataBase;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test.Domotica.Core
{
    [TestClass]
    public class UnitTestDataStore
    {
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


            var ok = await store.InsertAsync(json);
            Assert.IsTrue(ok);

            //var (okRead, readJson) = await store.ReadAsync(id);
            //Assert.IsTrue(okRead);

            //var storedJson = JsonConvert.SerializeObject(readJson);
            //var oldJson = JsonConvert.SerializeObject(JToken.Parse(json));

            //var equal = JToken.DeepEquals(storedJson, oldJson);
            //Assert.IsTrue(equal);

            //File.Delete($"{store.DataBaseName}.json");
        }
    }
}
