using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
{
    /// <summary>
    ///  Tests for the basic component.
    /// </summary>
    [TestClass]
    public class ComponentTests
    {
        /// <summary>
        ///  Stubbed out test component for testing.
        /// </summary>
        public class TestComponent : Component
        {
        }

        /// <summary>
        ///  Create a component, make sure everything is in the expected default state.
        /// </summary>
        [TestMethod]
        public void TestCreateComponent()
        {
            TestComponent c = new TestComponent();

            Assert.IsNull( c.Owner );
            Assert.AreEqual( Guid.Empty, c.Id );
            Assert.IsFalse( c.Enabled );
        }
    }
}
