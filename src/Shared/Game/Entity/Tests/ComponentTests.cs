using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Tests
{
    /// <summary>
    ///  Tests for the basic component.
    /// </summary>
    [TestFixture]
    [Category( "Entity" )]
    public class ComponentTests
    {
        /// <summary>
        ///  Stubbed out test component for testing.
        /// </summary>
        public class TestComponent : Component
        {
            public override void Update( Microsoft.Xna.Framework.GameTime time )
            {
                // empty
            }
        }

        /// <summary>
        ///  Create a component, make sure everything is in the expected default state.
        /// </summary>
        [Test]
        public void TestCreateComponent()
        {
            TestComponent c = new TestComponent();

            Assert.IsNull( c.Owner );
            Assert.AreEqual( Guid.Empty, c.Id );
            Assert.IsFalse( c.Enabled );
        }

        /// <summary>
        ///  Create a component, make sure everything is in the expected default state.
        /// </summary>
        [Test]
        public void TestRecycleComponent()
        {
            TestComponent c = new TestComponent();

            c.Owner = new GameObject( "blah", null );
            c.Id = Guid.NewGuid();
            c.Enabled = true;

            // make sure they are actually set
            Assert.IsNotNull( c.Owner );
            Assert.AreNotEqual( Guid.Empty, c.Id );
            Assert.IsTrue( c.Enabled );

            // reset the component, and see if the values have been reset
            c.Recycle();

            Assert.IsNull( c.Owner );
            Assert.AreEqual( Guid.Empty, c.Id );
            Assert.IsFalse( c.Enabled );
        }
    }
}
