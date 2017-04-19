using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void Remove_Prefix_From_String()
        {
            Assert.AreEqual("World", StringUtils.RemovePrefix("HelloWorld", "Hello"));
        }
    }
}
