using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class StringExtensionMethodsTests
    {
        [TestMethod]
        public void FormatWithOneArg()
        {
            Assert.AreEqual( "The answer is 42", "The answer is {0}".With( 42 ) );
        }

        [TestMethod]
        public void FormatWithTwoArgs()
        {
            Assert.AreEqual( "42 and True", "{0} and {1}".With( 42, true ) );
        }

        [TestMethod]
        public void FormatWithThreeArgs()
        {
            Assert.AreEqual( "42, True, 'Scott'", "{0}, {1}, '{2}'".With( 42, true, "Scott" ) );
        }

        [TestMethod]
        public void FormatWithFourArgs()
        {
            Assert.AreEqual( "42, True, 'Scott', 2.5",
                             "{0}, {1}, '{2}', {3}".With( 42, true, "Scott", 2.5f ) );
        }
    }
}
