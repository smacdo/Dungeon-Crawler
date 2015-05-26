using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameObjectTests
    {
        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void GameObjectCreatedWithNoNameHasEmptyName()
        {
            var go = new GameObject();
            Assert.AreEqual(string.Empty, go.Name);

            go = new GameObject(null);
            Assert.AreSame(string.Empty, go.Name);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void GameObjectNamedCtorSetsNameField()
        {
            var go = new GameObject("MyObject");
            Assert.AreEqual("MyObject", go.Name);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void GameObjectCtorAssignsNewGuids()
        {
            var a = new GameObject();
            var b = new GameObject();

            Assert.AreNotEqual(Guid.Empty, a.Id);
            Assert.AreNotEqual(Guid.Empty, b.Id);

            Assert.AreNotEqual(a.Id, b.Id);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void GameObjectComesWithTransformComponent()
        {
            var go = new GameObject();
            Assert.IsNotNull(go.Transform);
            Assert.AreSame(go.Transform, go.Get<TransformComponent>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void AddAndGetComponents()
        {
            var go = new GameObject();

            Assert.IsNull(go.Find<TestComponentA>());
            Assert.IsNull(go.Find<TestComponentB>());

            go.Add(new TestComponentA());
            go.Add(new TestComponentB());

            Assert.IsNotNull(go.Get<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullComponentThrowsException()
        {
            var go = new GameObject();
            go.Add<TestComponentA>(null);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        [ExpectedException(typeof(ComponentAlreadyAddedException))]
        public void AddComponentOfSameTypeTwiceThrowsException()
        {
            var go = new GameObject();

            go.Add(new TestComponentA());
            go.Add(new TestComponentA());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void AddingAndThenRemovingComponents()
        {
            var go = new GameObject();
            go.Add(new TestComponentA());
            go.Add(new TestComponentB());

            Assert.IsNotNull(go.Get<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove<TestComponentA>();
            Assert.IsNull(go.Find<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove<TestComponentB>();
            Assert.IsNull(go.Find<TestComponentA>());
            Assert.IsNull(go.Find<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void AddingAndThenRemovingByComponentType()
        {
            var go = new GameObject();
            go.Add(new TestComponentA());
            go.Add(new TestComponentB());

            Assert.IsNotNull(go.Get<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove(typeof(TestComponentA));
            Assert.IsNull(go.Find<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove(typeof(TestComponentB));
            Assert.IsNull(go.Find<TestComponentA>());
            Assert.IsNull(go.Find<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void FindReturnsAddedGameComponent()
        {
            var go = new GameObject();
            var a = new TestComponentA();
            var b = new TestComponentB();

            go.Add(a);
            go.Add(b);

            Assert.AreSame(a, go.Find<TestComponentA>());
            Assert.AreSame(b, go.Find<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        [ExpectedException(typeof(ComponentDoesNotExistException))]
        public void GetThrowsExceptionIfComponentWasNotAdded()
        {
            var go = new GameObject();
            go.Get<TestComponentA>();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void FindReturnsNullWhenComponentDoesNotExist()
        {
            var go = new GameObject();
            Assert.IsNull(go.Find<TestComponentA>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void GameObjectContainsWorksAsExpected()
        {
            var go = new GameObject();
            Assert.IsFalse(go.Contains<TestComponentA>());
            Assert.IsFalse(go.Contains<TestComponentB>());

            go.Add(new TestComponentA());
            Assert.IsTrue(go.Contains<TestComponentA>());
            Assert.IsFalse(go.Contains<TestComponentB>());

            go.Add(new TestComponentB());
            Assert.IsTrue(go.Contains<TestComponentA>());
            Assert.IsTrue(go.Contains<TestComponentB>());

            go.Remove<TestComponentA>();
            Assert.IsFalse(go.Contains<TestComponentA>());
            Assert.IsTrue(go.Contains<TestComponentB>());

            go.Remove<TestComponentB>();
            Assert.IsFalse(go.Contains<TestComponentA>());
            Assert.IsFalse(go.Contains<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void RemovingTransformSetsPropertyToNull()
        {
            var go = new GameObject();
            Assert.IsNotNull(go.Transform);

            go.Remove<TransformComponent>();
            Assert.IsNull(go.Transform);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void RemovingNonExistantComponentReturnsFalse()
        {
            var go = new GameObject();
            Assert.IsFalse(go.Remove<TestComponentA>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void RemovingComponentWillDisposeComponent()
        {
            var go = new GameObject();
            var dc = new DisposableTestComponent();

            Assert.AreEqual(0, dc.DisposeCalled);

            go.Add(dc);
            go.Remove<DisposableTestComponent>();

            Assert.AreEqual(1, dc.DisposeCalled);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void DisposeNullsOutFields()
        {
            var go = new GameObject();
            go.Dispose();

            Assert.AreEqual(Guid.Empty, go.Id);
            Assert.IsNull(go.Name);
            Assert.IsNull(go.Transform);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void DisposingGameObjectDisposesAllComponents()
        {
            var go = new GameObject();
            var a = new DisposableTestComponent();
            var b = new DisposableTestComponent2();

            go.Add(a);
            go.Add(b);

            Assert.AreEqual(0, a.DisposeCalled);
            Assert.AreEqual(0, b.DisposeCalled);

            go.Dispose();

            Assert.AreEqual(1, a.DisposeCalled);
            Assert.AreEqual(1, b.DisposeCalled);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        [ExpectedException(typeof(NullReferenceException))]
        public void AccessingComponentAfterDisposalThrowsException()
        {
            var go = new GameObject();
            go.Add(new TestComponentA());
            
            go.Dispose();

            go.Get<TestComponentA>();
        }

        /// <summary>
        ///  Test mocks.
        /// </summary>
        public class TestComponentA : Component
        {
            public TestComponentA()
            {
            }
        }

        public class TestComponentB: Component
        {
            public TestComponentB()
            {
            }
        }

        public class DisposableTestComponent : IComponent
        {
            public int DisposeCalled { get; private set;  }

            public DisposableTestComponent()
            {
                DisposeCalled = 0;
            }

            void IDisposable.Dispose()
            {
                DisposeCalled++;
            }

            public IGameObject Owner { get; set; }
        }

        public class DisposableTestComponent2 : IComponent
        {
            public int DisposeCalled { get; private set; }

            public DisposableTestComponent2()
            {
                DisposeCalled = 0;
            }

            void IDisposable.Dispose()
            {
                DisposeCalled++;
            }

            public IGameObject Owner { get; set; }
        }
    }
}
