using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;
using Scott.Forge.Tests.GameObjectsTests;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class PooledComponentProcessorTests
    {
        TestComponentProcessor mProcessor;

        [TestInitialize]
        public void SetUp()
        {
            mProcessor = new TestComponentProcessor();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        public void AddGameObjectsIncrementsCount()
        {
            Assert.AreEqual(0, mProcessor.GameObjectCount);

            mProcessor.Add(new GameObject());
            Assert.AreEqual(1, mProcessor.GameObjectCount);

            mProcessor.Add(new GameObject());
            Assert.AreEqual(2, mProcessor.GameObjectCount);
        }


        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        public void AddGameObjectToProcessorAddsComponent()
        {
            var gameObject = new GameObject();
            Assert.IsFalse(gameObject.Contains<PooledTestComponent>());

            var component = mProcessor.Add(gameObject);

            Assert.IsTrue(gameObject.Contains<PooledTestComponent>());
            Assert.AreSame(component, gameObject.Get<PooledTestComponent>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        public void AddTwoGameObjectsToProcessorAddsComponents()
        {
            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();

            var component1 = mProcessor.Add(gameObject1);
            var component2 = mProcessor.Add(gameObject2);

            Assert.AreSame(component1, gameObject1.Get<PooledTestComponent>());
            Assert.AreSame(component2, gameObject2.Get<PooledTestComponent>());
            Assert.AreNotSame(component1, component2);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullGameObjectThrowsException()
        {
            mProcessor.Add(null);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        public void AddAndRemoveGameObjectsFromProcessor()
        {
            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();

            // Add two game objects.
            var component1 = mProcessor.Add(gameObject1);
            var component2 = mProcessor.Add(gameObject2);

            Assert.AreEqual(2, mProcessor.GameObjectCount);
            Assert.AreSame(component1, gameObject1.Get<PooledTestComponent>());
            Assert.AreSame(component2, gameObject2.Get<PooledTestComponent>());

            // Remove first game object, make sure component was removed.
            mProcessor.Remove(gameObject1);
            Assert.IsFalse(gameObject1.Contains<TestComponent>());
            Assert.AreEqual(1, mProcessor.GameObjectCount);

            // Add game object back, make sure game object count is correct.
            var component1b = mProcessor.Add(gameObject1);

            Assert.AreEqual(2, mProcessor.GameObjectCount);
            Assert.AreSame(component1b, gameObject1.Get<PooledTestComponent>());
            Assert.AreSame(component2, gameObject2.Get<PooledTestComponent>());
            Assert.AreNotSame(component1, component1b);

            // Remove the second game object.
            mProcessor.Remove(gameObject2);
            Assert.IsFalse(gameObject2.Contains<TestComponent>());
            Assert.AreEqual(1, mProcessor.GameObjectCount);

            // Remove the first game object.
            mProcessor.Remove(gameObject1);
            Assert.IsFalse(gameObject1.Contains<TestComponent>());
            Assert.AreEqual(0, mProcessor.GameObjectCount);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveNullGameObjectThrowsException()
        {
            mProcessor.Remove(null);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        [ExpectedException(typeof(ComponentDoesNotExistException))]
        public void RemoveGameObjectThatWasNotAddedThrowsException()
        {
            mProcessor.Remove(new GameObject());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/PooledComponentProcessor")]
        public void UpdateGameObjects()
        {
            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();

            // Add with no game objects.
            mProcessor.Update(1.0f, 1.0f);
            Assert.AreEqual(0, mProcessor.UpdatedComponents.Count);

            // Update with one game object.
            var component1 = mProcessor.Add(gameObject1);
            mProcessor.Update(2.5f, 1.5f);

            Assert.AreEqual(1, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(2.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(1.5f, mProcessor.UpdatedComponents[component1].DeltaTime);

            // Update with two game objects.
            var component2 = mProcessor.Add(gameObject2);
            mProcessor.Update(4.5f, 2.0f);

            Assert.AreEqual(2, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component1].DeltaTime);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component2].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component2].DeltaTime);

            // Update with one game object removed.
            mProcessor.Remove(gameObject1);
            mProcessor.Update(7.5f, 3.0f);

            Assert.AreEqual(2, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component1].DeltaTime);
            Assert.AreEqual(7.5f, mProcessor.UpdatedComponents[component2].CurrentTime);
            Assert.AreEqual(3.0f, mProcessor.UpdatedComponents[component2].DeltaTime);

            // Update with all game objects removed.
            mProcessor.Remove(gameObject2);
            mProcessor.Update(17.5f, 10.0f);

            Assert.AreEqual(2, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component1].DeltaTime);
            Assert.AreEqual(7.5f, mProcessor.UpdatedComponents[component2].CurrentTime);
            Assert.AreEqual(3.0f, mProcessor.UpdatedComponents[component2].DeltaTime);
        }

        /// <summary>
        ///  Testable and recyclable version of the test component.
        /// </summary>
        private class PooledTestComponent : TestComponent, IRecyclable
        {
            public void Recycle()
            {
            }
        }

        /// <summary>
        ///  Testable version of the component processor.
        /// </summary>
        private class TestComponentProcessor : PooledComponentProcessor<PooledTestComponent>
        {
            public TestComponentProcessor()
            {
                UpdatedComponents = new Dictionary<PooledTestComponent, TimeInfo>();
            }

            /// <summary>
            ///  Contains list of components that were updated in last call to Update, along with the parameters sent
            ///  to the method invocation.
            /// </summary>
            public Dictionary<PooledTestComponent, TimeInfo> UpdatedComponents;

            /// <summary>
            ///  Mark each component with the time provided as being updated.
            /// </summary>
            protected override void UpdateComponent(PooledTestComponent component, double currentTime, double deltaTime)
            {
                UpdatedComponents[component] = new TimeInfo { CurrentTime = currentTime, DeltaTime = deltaTime }; ;
            }

            /// <summary>
            ///  Holds information on when a component was last updated.
            /// </summary>
            public struct TimeInfo
            {
                public double CurrentTime;
                public double DeltaTime;
            }
        }
    }
}
