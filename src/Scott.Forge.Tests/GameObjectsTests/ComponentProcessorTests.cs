using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
{
    /// <summary>
    /// </summary>
    /// <remarks>
    ///  TODO: Test that game components are recycled properly.
    /// </remarks>
    [TestClass]
    public class ComponentProcessorTests
    {
        TestComponentProcessor mProcessor;
              
        [TestInitialize]
        public void SetUp()
        {
            mProcessor = new TestComponentProcessor();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
        public void AddGameObjectsIncrementsCount()
        {
            Assert.AreEqual(0, mProcessor.ComponetnCount);

            mProcessor.Add(new GameObject());
            Assert.AreEqual(1, mProcessor.ComponetnCount);

            mProcessor.Add(new GameObject());
            Assert.AreEqual(2, mProcessor.ComponetnCount);
        }


        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
        public void AddGameObjectToProcessorAddsComponent()
        {
            var gameObject = new GameObject();
            Assert.IsFalse(gameObject.Contains<TestComponent>());

            var component = mProcessor.Add(gameObject);

            Assert.IsTrue(gameObject.Contains<TestComponent>());
            Assert.AreSame(component, gameObject.Get<TestComponent>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
        public void AddTwoGameObjectsToProcessorAddsComponents()
        {
            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();

            var component1 = mProcessor.Add(gameObject1);
            var component2 = mProcessor.Add(gameObject2);

            Assert.AreSame(component1, gameObject1.Get<TestComponent>());
            Assert.AreSame(component2, gameObject2.Get<TestComponent>());
            Assert.AreNotSame(component1, component2);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
        public void AddAndRemoveGameObjectsFromProcessor()
        {
            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();

            // Add two game objects.
            var component1 = mProcessor.Add(gameObject1);
            var component2 = mProcessor.Add(gameObject2);

            Assert.AreEqual(2, mProcessor.ComponetnCount);
            Assert.AreSame(component1, gameObject1.Get<TestComponent>());
            Assert.AreSame(component2, gameObject2.Get<TestComponent>());

            // Remove first game object, make sure component was removed.
            Assert.IsTrue(mProcessor.Remove(component1));
            Assert.IsFalse(gameObject1.Contains<TestComponent>());
            Assert.AreEqual(1, mProcessor.ComponetnCount);

            // Add game object back, make sure game object count is correct.
            var component1b = mProcessor.Add(gameObject1);

            Assert.AreEqual(2, mProcessor.ComponetnCount);
            Assert.AreSame(component1b, gameObject1.Get<TestComponent>());
            Assert.AreSame(component2, gameObject2.Get<TestComponent>());
            Assert.AreNotSame(component1, component1b);

            // Remove the second game object.
            Assert.IsTrue(mProcessor.Remove(component2));
            Assert.IsFalse(gameObject2.Contains<TestComponent>());
            Assert.AreEqual(1, mProcessor.ComponetnCount);

            // Remove the first game object.
            Assert.IsTrue(mProcessor.Remove(component1b));
            Assert.IsFalse(gameObject1.Contains<TestComponent>());
            Assert.AreEqual(0, mProcessor.ComponetnCount);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
        public void RemovingNotAddedComponentReturnsFalse()
        {
            Assert.IsFalse(mProcessor.Remove(null));
            Assert.IsFalse(mProcessor.Remove(new TestComponent()));
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/ComponentProcessor")]
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
            mProcessor.Remove(component1);
            mProcessor.Update(7.5f, 3.0f);

            Assert.AreEqual(2, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component1].DeltaTime);
            Assert.AreEqual(7.5f, mProcessor.UpdatedComponents[component2].CurrentTime);
            Assert.AreEqual(3.0f, mProcessor.UpdatedComponents[component2].DeltaTime);

            // Update with all game objects removed.
            mProcessor.Remove(component2);
            mProcessor.Update(17.5f, 10.0f);

            Assert.AreEqual(2, mProcessor.UpdatedComponents.Count);
            Assert.AreEqual(4.5f, mProcessor.UpdatedComponents[component1].CurrentTime);
            Assert.AreEqual(2.0f, mProcessor.UpdatedComponents[component1].DeltaTime);
            Assert.AreEqual(7.5f, mProcessor.UpdatedComponents[component2].CurrentTime);
            Assert.AreEqual(3.0f, mProcessor.UpdatedComponents[component2].DeltaTime);
        }

        /// <summary>
        ///  Testable version of the component processor.
        /// </summary>
        private class TestComponentProcessor : ComponentProcessor<TestComponent>
        {
            public TestComponentProcessor()
            {
                UpdatedComponents = new Dictionary<TestComponent, TimeInfo>();
            }

            /// <summary>
            ///  Contains list of components that were updated in last call to Update, along with the parameters sent
            ///  to the method invocation.
            /// </summary>
            public Dictionary<TestComponent, TimeInfo> UpdatedComponents;

            /// <summary>
            ///  Mark each component with the time provided as being updated.
            /// </summary>
            protected override void UpdateComponent(TestComponent component, double currentTime, double deltaTime)
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
