using Domotica.Core.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domotica.Core
{
    [TestClass]
    public class UnitTestIdentity
    {
        [TestMethod]
        public void TestReadToken()
        {
            var token = new Token().Generate();

            Assert.IsNotNull(token);    
        }
    }
}
