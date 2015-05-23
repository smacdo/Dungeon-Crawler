using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
{
    /// <summary>
    ///  Tests for the basic component.
    /// </summary>
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ComponentTests
    {
        [TestMethod]
        [TestCategory("Forge/GameObjects/Component")]
        public void CanSetOwnerAfterConstruction()
        {
            var c = new TestComponent();
            Assert.IsNull(c.Owner);

            var go = new GameObject();
            c.Owner = go;

            Assert.AreSame(c.Owner, go);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/Component")]
        [ExpectedException(typeof(CannotChangeComponentOwnerException))]
        public void CannotChangeOwnerOnceSet()
        {
            var c = new TestComponent {Owner = new TestGameObject()};
            c.Owner = new TestGameObject();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/Component")]
        [ExpectedException(typeof(CannotChangeComponentOwnerException))]
        public void CreateComponentWithFactory()
        {
            var c = new TestComponent { Owner = new TestGameObject() };
            c.Owner = new TestGameObject();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/Component")]
        public void DisposedComponentInformsFactoryOfDestruction()
        {
            var f = new ComponentDestroyedCallback();
            var c = new TestComponent(f);
            
            ((IDisposable) c).Dispose();

            Assert.AreSame(c, f.ComponentsDestroyed.Single());
        }

        /// <summary>
        ///  Stubbed out test component for testing.
        /// </summary>
        public class TestComponent : Component
        {
            public TestComponent()
            {
            }

            public TestComponent(IComponentDestroyedCallback callback)
                : base(callback)
            {
            }
        }

        public class ComponentDestroyedCallback : IComponentDestroyedCallback
        {
            public List<IComponent> ComponentsDestroyed { get; set; }

            public ComponentDestroyedCallback()
            {
                ComponentsDestroyed = new List<IComponent>();
            }

            public void Destroy(IComponent component)
            {
                ComponentsDestroyed.Add(component);
            }
        }

        public class TestGameObject : IGameObject
        {
            public void Add<T>(T component) where T : IComponent
            {
                throw new NotImplementedException();
            }

            public bool Remove<T>() where T : IComponent
            {
                throw new NotImplementedException();
            }

            public string DumpDebugInfoToString()
            {
                throw new NotImplementedException();
            }

            public T Get<T>() where T : IComponent
            {
                throw new NotImplementedException();
            }

            public bool Contains<T>() where T : IComponent
            {
                throw new NotImplementedException();
            }

            public Guid Id
            {
                get { throw new NotImplementedException(); }
            }

            public string Name
            {
                get { throw new NotImplementedException(); }
            }

            public bool Enabled
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

            public TransformComponent Transform
            {
                get { throw new NotImplementedException(); }
            }


            public TComponent Find<TComponent>() where TComponent : class, IComponent
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}
