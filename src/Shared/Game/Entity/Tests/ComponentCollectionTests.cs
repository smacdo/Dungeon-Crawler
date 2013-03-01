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
            public int UpdateCounter = 0;

            public override void Update( Microsoft.Xna.Framework.GameTime time )
            {
                UpdateCounter++;
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

        [Test]
        public void TestUpdateLoop()
        {
            ComponentCollection<TestComponent> collection = new ComponentCollection<TestComponent>();

            // Create a game object
            TestComponent a = collection.Create( gameObject );
            TestComponent b = collection.Create( new GameObject() );

            // Make sure they ahve no updates
            Assert.AreEqual( 0, a.UpdateCounter );
            Assert.AreEqual( 0, b.UpdateCounter );

            // Update the collection and see if they got an update
            collection.Update( new Microsoft.Xna.Framework.GameTime() );

            Assert.AreEqual( 1, a.UpdateCounter );
            Assert.AreEqual( 1, b.UpdateCounter );

            // Update the collection and see if they got an update
            collection.Update( new Microsoft.Xna.Framework.GameTime() );

            Assert.AreEqual( 2, a.UpdateCounter );
            Assert.AreEqual( 2, b.UpdateCounter );
        }
    }
}
