#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common.Tests
{
    [TestFixture]
    public class StringExtensionMethodsTests
    {
        [Test]
        public void FormatWithOneArg()
        {
            Assert.AreEqual( "The answer is 42", "The answer is {0}".With( 42 ) );
        }

        [Test]
        public void FormatWithTwoArgs()
        {
            Assert.AreEqual( "42 and True", "{0} and {1}".With( 42, true ) );
        }

        [Test]
        public void FormatWithThreeArgs()
        {
            Assert.AreEqual( "42, True, 'Scott'", "{0}, {1}, '{2}'".With( 42, true, "Scott" ) );
        }

        [Test]
        public void FormatWithFourArgs()
        {
            Assert.AreEqual( "42, True, 'Scott', 2.5",
                             "{0}, {1}, '{2}', {3}".With( 42, true, "Scott", 2.5f ) );
        }
    }
}
