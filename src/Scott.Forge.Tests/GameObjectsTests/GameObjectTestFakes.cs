using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
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

    public class TestGameObject : IGameObject
    {
        private Guid mId;

        public TestGameObject(Guid? id = null)
        {
            mId = id ?? Guid.NewGuid();
        }

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
            get { return mId; }
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
