using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Tests.GameObjectsTests
{
    [TestClass]
    public class TransformComponentTests
    {
        [TestMethod]
        [TestCategory("Forge/GameObjects/TransformComponent")]
        public void CreateComponentSetsDefaultProperties()
        {
            var tc = new TransformComponent();

            Assert.AreEqual(TransformComponent.DefaultDirection, tc.Direction);
            Assert.AreEqual(TransformComponent.DefaultScale, tc.Scale);
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/TransformComponent")]
        public void GetAndSetPosition()
        {
            var tc = new TransformComponent();
            tc.Position = new Vector2(2.0f, 3.0f);

            Assert.AreEqual(new Vector2(2.0f, 3.0f), tc.Position);
        }
    }
}
