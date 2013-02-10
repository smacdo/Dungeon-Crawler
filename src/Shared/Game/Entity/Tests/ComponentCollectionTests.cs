using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game.Entity.Tests
{
    /// <summary>
    ///  Unit tests for ComponentCollection.
    /// </summary>
    [TestFixture]
    [Category( "Entity" )]
    public class ComponentCollectionTests
    {
        private GameObject gameObject;

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

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
        }

        [TearDown]
        public void TearDown()
        {
//            gameObject.Dispose();
            gameObject = null;
        }

        [Test]
        public void TestCreateComponentCollection()
        {
            ComponentCollection<TestComponent> collection = new ComponentCollection<TestComponent>();
        }

        [Test]
        public void TestCreateSingleComponent()
        {
            ComponentCollection<TestComponent> collection = new ComponentCollection<TestComponent>();

            // Create a game object
            TestComponent c = collection.Create( gameObject );

            // Test basic properties first
            Assert.IsNotNull( c.Owner );
            Assert.AreSame( gameObject, c.Owner );
            Assert.IsTrue( c.Enabled );
            Assert.AreNotEqual( Guid.Empty, c.Id );
        }

        [Test]
        public void TestCreateUsingIComponentInterface()
        {
            ComponentCollection<TestComponent> cc = new ComponentCollection<TestComponent>();
            IComponentCollection collection = cc;

            // Create a game object
            IComponent c = collection.Create( gameObject );

            // Test basic properties first
            Assert.IsNotNull( c.Owner );
            Assert.AreSame( gameObject, c.Owner );
            Assert.IsTrue( c.Enabled );
            Assert.AreNotEqual( Guid.Empty, c.Id );

            // Can we cast this?
            TestComponent testC = c as TestComponent;

            Assert.IsNotNull( testC );
        }
    }
}
