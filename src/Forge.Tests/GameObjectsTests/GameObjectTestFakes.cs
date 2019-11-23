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

        public string DumpDebugInfoToString()
        {
            throw new NotImplementedException();
        }
    }
}
