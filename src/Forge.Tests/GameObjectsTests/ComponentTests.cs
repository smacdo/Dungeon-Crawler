using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.GameObjects;

namespace Forge.Tests.GameObjectsTests
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
    }
}
