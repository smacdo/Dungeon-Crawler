using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.GameObjects;

namespace Forge.Tests.GameObjectsTests
{
    [TestClass]
    public class TransformComponentTests
    {
        [TestMethod]
        public void NewTransformComponentHasValidDefaultValues()
        {
            var t = new TransformComponent();
        }

        [TestMethod]
        [TestCategory("Forge/GameObjects/TransformComponent")]
        public void GetAndSetPosition()
        {
            var tc = new TransformComponent();
            tc.WorldPosition = new Vector2(2.0f, 3.0f);

            Assert.AreEqual(new Vector2(2.0f, 3.0f), tc.WorldPosition);
        }
    }
}
