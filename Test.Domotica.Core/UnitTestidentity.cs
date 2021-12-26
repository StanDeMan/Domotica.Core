using System;
using System.Diagnostics;
using Domotica.Core.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domotica.Core
{
    [TestClass]
    public sealed class UnitTestIdentity
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

        [TestMethod]
        public void TestGenerateWithDigLength()
        {
            var token = new Token().Generate(8);

            Assert.IsNotNull(token);  
            Assert.AreEqual(8, token.Length);  // set to 8 chars
        }

        [TestMethod]
        public void TestGenerateWithArgumentTooLowException()
        {
            try
            {
                new Token().Generate(7);
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
                Assert.AreNotEqual(null, e);
            }
        }

        [TestMethod]
        public void TestGenerateWithArgumentTooHighException()
        {
            try
            {
                new Token().Generate(13);
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e);
                Assert.AreNotEqual(null, e);
            }
        }
    }
}
