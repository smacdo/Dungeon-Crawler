using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forge.GameObjects;

namespace Forge.Tests.GameObjectsTests
{
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

    public class TestGameObject : GameObject
    {
        private Guid mId;

        public TestGameObject(Guid? id = null)
        {
            mId = id ?? Guid.NewGuid();
        }

        public T Add<T>(T component) where T : IComponent
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
            get { return mId; }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

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

        public TransformComponent Transform
        {
            get { throw new NotImplementedException(); }
        }

        public GameObject Parent
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

        public GameObject FirstChild
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GameObject NextSibling
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ActiveInHierarchy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ActiveSelf
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TComponent TryGet<TComponent>() where TComponent : class, IComponent
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public GameObject FindChildByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
