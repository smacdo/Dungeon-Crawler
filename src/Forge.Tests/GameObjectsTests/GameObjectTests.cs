using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.GameObjects;

namespace Forge.Tests.GameObjectsTests
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

            Assert.IsNull(go.TryGet<TestComponentA>());
            Assert.IsNull(go.TryGet<TestComponentB>());

            go.Add(new TestComponentA());
            go.Add(new TestComponentB());

            Assert.IsNotNull(go.Get<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void AddReturnsInstanceOfAddedComponetn()
        {
            var go = new GameObject();
            var a = new TestComponentA();
            var b = new TestComponentB();

            Assert.AreSame(a, go.Add(a));
            Assert.AreSame(b, go.Add(b));
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
            Assert.IsNull(go.TryGet<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove<TestComponentB>();
            Assert.IsNull(go.TryGet<TestComponentA>());
            Assert.IsNull(go.TryGet<TestComponentB>());
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
            Assert.IsNull(go.TryGet<TestComponentA>());
            Assert.IsNotNull(go.Get<TestComponentB>());

            go.Remove(typeof(TestComponentB));
            Assert.IsNull(go.TryGet<TestComponentA>());
            Assert.IsNull(go.TryGet<TestComponentB>());
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        public void Add_Assigns_Component_Owner()
        {
            var go = new GameObject();
            var a = new TestComponentA();

            Assert.IsNull(a.Owner);

            go.Add(a);
            Assert.AreSame(go, a.Owner);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/GameObject")]
        [ExpectedException(typeof(CannotChangeComponentOwnerException))]
        public void Add_Throws_Exception_If_Component_Has_Another_Owner()
        {
            var prevGameObject = new GameObject();
            var a = prevGameObject.Add(new TestComponentA());

            Assert.AreSame(prevGameObject, a.Owner);

            var newGameObject = new GameObject();
            newGameObject.Add(a);
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

            Assert.AreSame(a, go.TryGet<TestComponentA>());
            Assert.AreSame(b, go.TryGet<TestComponentB>());
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
            Assert.IsNull(go.TryGet<TestComponentA>());
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

        [TestMethod]
        public void NewGameObjectHasCorrectChildDefaults()
        {
            var obj = new GameObject();

            Assert.IsNull(obj.Parent);
            Assert.IsNull(obj.FirstChild);
            Assert.IsNull(obj.NextSibling);
        }

        [TestMethod]
        public void AddingChildCorrectlyParentsEverything()
        {
            var parent = new GameObject();

            // Create and add one child.
            var firstChild = new GameObject();
            firstChild.Parent = parent;

            Assert.IsNull(parent.Parent);
            Assert.IsNull(parent.NextSibling);
            Assert.AreSame(parent.FirstChild, firstChild);

            Assert.AreSame(firstChild.Parent, parent);
            Assert.IsNull(firstChild.NextSibling);
            Assert.IsNull(firstChild.FirstChild);

            // Add second child.
            var secondChild = new GameObject();
            secondChild.Parent = parent;

            Assert.IsNull(parent.Parent);
            Assert.IsNull(parent.NextSibling);
            Assert.AreSame(parent.FirstChild, secondChild);

            Assert.AreSame(secondChild.Parent, parent);
            Assert.AreSame(secondChild.NextSibling, firstChild);
            Assert.IsNull(secondChild.FirstChild);

            Assert.AreSame(firstChild.Parent, parent);
            Assert.IsNull(firstChild.NextSibling);
            Assert.IsNull(firstChild.FirstChild);
        }

        [TestMethod]
        public void MovingGameObjectFromOneParentToAnother()
        {
            var firstParent = new GameObject();

            // Add child.
            var child = new GameObject();
            child.Parent = firstParent;

            Assert.IsNull(firstParent.Parent);
            Assert.IsNull(firstParent.NextSibling);
            Assert.AreSame(firstParent.FirstChild, child);

            Assert.AreSame(child.Parent, firstParent);
            Assert.IsNull(child.NextSibling);
            Assert.IsNull(child.FirstChild);

            // Move child to new game object.
            var secondParent = new GameObject();
            child.Parent = secondParent;

            Assert.IsNull(firstParent.Parent);
            Assert.IsNull(firstParent.NextSibling);
            Assert.IsNull(firstParent.FirstChild);

            Assert.IsNull(secondParent.Parent);
            Assert.IsNull(secondParent.NextSibling);
            Assert.AreSame(secondParent.FirstChild, child);

            Assert.AreSame(child.Parent, secondParent);
            Assert.IsNull(child.NextSibling);
            Assert.IsNull(child.FirstChild);
        }

        [TestMethod]
        public void MovingTwoGameObjectFromOneParentToAnother()
        {
            var firstParent = new GameObject();

            // Add two children.
            var firstChild = new GameObject();
            firstChild.Parent = firstParent;

            var secondChild = new GameObject();
            secondChild.Parent = firstParent;

            Assert.IsNull(firstParent.Parent);
            Assert.IsNull(firstParent.NextSibling);
            Assert.AreSame(firstParent.FirstChild, secondChild);

            Assert.AreSame(secondChild.Parent, firstParent);
            Assert.AreSame(secondChild.NextSibling, firstChild);
            Assert.IsNull(secondChild.FirstChild);

            Assert.AreSame(firstChild.Parent, firstParent);
            Assert.IsNull(firstChild.NextSibling);
            Assert.IsNull(firstChild.FirstChild);

            // Move the first child to the new parent.
            var secondParent = new GameObject();
            firstChild.Parent = secondParent;

            Assert.IsNull(firstParent.Parent);
            Assert.IsNull(firstParent.NextSibling);
            Assert.AreSame(firstParent.FirstChild, secondChild);

            Assert.IsNull(secondParent.Parent);
            Assert.IsNull(secondParent.NextSibling);
            Assert.AreSame(secondParent.FirstChild, firstChild);

            Assert.AreSame(firstChild.Parent, secondParent);
            Assert.IsNull(firstChild.NextSibling);
            Assert.IsNull(firstChild.FirstChild);

            Assert.AreSame(secondChild.Parent, firstParent);
            Assert.IsNull(secondChild.NextSibling);
            Assert.IsNull(secondChild.FirstChild);

            // Move the second second to the new parent.
            secondChild.Parent = secondParent;

            Assert.IsNull(firstParent.Parent);
            Assert.IsNull(firstParent.NextSibling);
            Assert.IsNull(firstParent.FirstChild);

            Assert.IsNull(secondParent.Parent);
            Assert.IsNull(secondParent.NextSibling);
            Assert.AreSame(secondParent.FirstChild, secondChild);

            Assert.AreSame(firstChild.Parent, secondParent);
            Assert.IsNull(firstChild.NextSibling);
            Assert.IsNull(firstChild.FirstChild);

            Assert.AreSame(secondChild.Parent, secondParent);
            Assert.AreSame(secondChild.NextSibling, firstChild);
            Assert.IsNull(secondChild.FirstChild);
        }

        [TestMethod]
        public void ChangeGameObjectParentToNull()
        {
            var parent = new GameObject();

            // Add child.
            var child = new GameObject();
            child.Parent = parent;

            Assert.IsNull(parent.Parent);
            Assert.IsNull(parent.NextSibling);
            Assert.AreSame(parent.FirstChild, child);

            Assert.AreSame(child.Parent, parent);
            Assert.IsNull(child.NextSibling);
            Assert.IsNull(child.FirstChild);

            // Set child game object to null.
            child.Parent = null;

            Assert.IsNull(parent.Parent);
            Assert.IsNull(parent.NextSibling);
            Assert.IsNull(parent.FirstChild);

            Assert.IsNull(child.Parent);
            Assert.IsNull(child.NextSibling);
            Assert.IsNull(child.FirstChild);
        }

        [TestMethod]
        public void FindChildByName()
        {
            var root = new GameObject("Root");

            // Make sure when no children attached everything works.
            Assert.IsNull(root.FindChildByName("FirstChild"));

            // Attach a single child and make sure that object is found.
            var firstChild = new GameObject("FirstChild");
            firstChild.Parent = root;

            Assert.AreSame(firstChild, root.FindChildByName("FirstChild"));
            Assert.IsNull(root.FindChildByName("OopsIDoNotExist"));

            // Attach a second child with a different name make sure both are found.
            var secondChild = new GameObject("SecondChild");
            secondChild.Parent = root;

            Assert.AreSame(firstChild, root.FindChildByName("FirstChild"));
            Assert.AreSame(secondChild, root.FindChildByName("SecondChild"));

            // Attach third child that has same name as the first child and make sure it comes first.
            // (duplicate name, children are ordered by time attached in reverse order).
            var firstChildDuplicate = new GameObject("FirstChild");
            firstChildDuplicate.Parent = root;

            Assert.AreSame(firstChildDuplicate, root.FindChildByName("FirstChild"));
            Assert.AreSame(secondChild, root.FindChildByName("SecondChild"));

            // Add a named child to the second child to test that find recursively searches children.
            var secondChildFirstChild = new GameObject("SecondChildFirstChild");
            secondChildFirstChild.Parent = secondChild;

            Assert.AreSame(secondChildFirstChild, root.FindChildByName("SecondChildFirstChild"));
            Assert.AreSame(firstChildDuplicate, root.FindChildByName("FirstChild"));
            Assert.AreSame(secondChild, root.FindChildByName("SecondChild"));

            // Ensure that the find does not search the starting node or its parent.
            var newParent = new GameObject("Root");     // Use same name.
            root.Parent = newParent;

            Assert.IsNull(root.FindChildByName("Root"));
        }

        [TestMethod]
        public void SetActiveChangesActiveSelfProperty()
        {
            var o = new GameObject();
            Assert.AreEqual(o.Active, o.ActiveSelf);

            o.Active = true;
            Assert.IsTrue(o.ActiveSelf);

            o.Active = false;
            Assert.IsFalse(o.ActiveSelf);
        }

        [TestMethod]
        public void SetActiveCorrectlyPropagatesChanges()
        {
            var parent = new GameObject();
            var obj = new GameObject();
            var first = new GameObject();
            var second = new GameObject();

            obj.Parent = parent;
            first.Parent = obj;
            second.Parent = obj;

            // Check defaults.
            Assert.IsTrue(parent.Active);
            Assert.IsTrue(obj.Active);
            Assert.IsTrue(first.Active);
            Assert.IsTrue(second.Active);

            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsTrue(obj.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveInHierarchy);

            Assert.IsTrue(parent.ActiveSelf);
            Assert.IsTrue(obj.ActiveSelf);
            Assert.IsTrue(first.ActiveSelf);
            Assert.IsTrue(second.ActiveSelf);

            // Set obj to inactive will deactivate both children.
            obj.Active = false;

            Assert.IsTrue(parent.Active);
            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsTrue(parent.ActiveSelf);

            Assert.IsFalse(obj.Active);
            Assert.IsTrue(obj.ActiveInHierarchy);
            Assert.IsFalse(obj.ActiveSelf);

            Assert.IsFalse(first.Active);
            Assert.IsFalse(first.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveSelf);

            Assert.IsFalse(second.Active);
            Assert.IsFalse(second.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveSelf);

            // Disable the parent which will deactivate all children.
            parent.Active = false;

            Assert.IsFalse(parent.Active);
            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsFalse(parent.ActiveSelf);

            Assert.IsFalse(obj.Active);
            Assert.IsFalse(obj.ActiveInHierarchy);
            Assert.IsFalse(obj.ActiveSelf);

            Assert.IsFalse(first.Active);
            Assert.IsFalse(first.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveSelf);

            Assert.IsFalse(second.Active);
            Assert.IsFalse(second.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveSelf);

            // Enable obj and ensure that all children all still inactive.
            obj.Active = true;

            Assert.IsFalse(parent.Active);
            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsFalse(parent.ActiveSelf);

            Assert.IsFalse(obj.Active);
            Assert.IsFalse(obj.ActiveInHierarchy);
            Assert.IsTrue(obj.ActiveSelf);

            Assert.IsFalse(first.Active);
            Assert.IsFalse(first.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveSelf);

            Assert.IsFalse(second.Active);
            Assert.IsFalse(second.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveSelf);

            // Disable obj and then re-enable the parent. This should activate the parent but prevent active from
            // spreading to obj or its children. 
            obj.Active = false;
            parent.Active = true;

            Assert.IsTrue(parent.Active);
            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsTrue(parent.ActiveSelf);

            Assert.IsFalse(obj.Active);
            Assert.IsTrue(obj.ActiveInHierarchy);
            Assert.IsFalse(obj.ActiveSelf);

            Assert.IsFalse(first.Active);
            Assert.IsFalse(first.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveSelf);

            Assert.IsFalse(second.Active);
            Assert.IsFalse(second.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveSelf);

            // Now re-enable obj which makes all objects active again.
            obj.Active = true;

            Assert.IsTrue(parent.Active);
            Assert.IsTrue(parent.ActiveInHierarchy);
            Assert.IsTrue(parent.ActiveSelf);

            Assert.IsTrue(obj.Active);
            Assert.IsTrue(obj.ActiveInHierarchy);
            Assert.IsTrue(obj.ActiveSelf);

            Assert.IsTrue(first.Active);
            Assert.IsTrue(first.ActiveInHierarchy);
            Assert.IsTrue(first.ActiveSelf);

            Assert.IsTrue(second.Active);
            Assert.IsTrue(second.ActiveInHierarchy);
            Assert.IsTrue(second.ActiveSelf);
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

            public void OnOwnerEnableChanged(bool isOwnerEnabled)
            {
                throw new NotImplementedException();
            }

            public GameObject Owner { get; set; }

            public bool Active
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
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

            public void OnOwnerEnableChanged(bool isOwnerEnabled)
            {
                throw new NotImplementedException();
            }

            public GameObject Owner { get; set; }

            public bool Active
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
