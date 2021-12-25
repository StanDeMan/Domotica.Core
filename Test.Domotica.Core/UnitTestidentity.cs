using Domotica.Core.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domotica.Core
{
    [TestClass]
    public class UnitTestIdentity
    {
        [TestMethod]
        public void TestGenerateToken()
        {
            var token = new Token().Generate();

            Assert.IsNotNull(token);  
            Assert.AreEqual(12, token.Length);  // default: set to 12 chars
        }

        [TestMethod]
        public void TestGenerateWithSeedToken()
        {
            var token = new Token(12345678).Generate();

            Assert.IsNotNull(token);  
            Assert.AreEqual(12, token.Length);  // default: set to 12 chars
        }
    }
}
