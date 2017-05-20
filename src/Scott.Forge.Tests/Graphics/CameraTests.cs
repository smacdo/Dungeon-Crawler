using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Graphics;

namespace Scott.Forge.Tests.Graphics
{
    [TestClass]
    public class CameraTests
    {
        [TestMethod]
        public void Create_New_Camera_Default_Constructor_Sets_Values_To_Zero()
        {
            var camera = new Camera();
            Assert.AreEqual(SizeF.Empty, camera.Viewport);
            Assert.AreEqual(Vector2.Zero, camera.Position);
        }

        [TestMethod]
        public void Create_New_Camera_With_Viewport_Only_Sets_Position_To_Zero()
        {
            var camera = new Camera(new SizeF(800, 600));
            Assert.AreEqual(new SizeF(800, 600), camera.Viewport);
            Assert.AreEqual(Vector2.Zero, camera.Position);
        }

        [TestMethod]
        public void Create_New_Camera_With_Viewport_And_Position()
        {
            var camera = new Camera(new SizeF(640, 480), new Vector2(-100, 120));
            Assert.AreEqual(new SizeF(640, 480), camera.Viewport);
            Assert.AreEqual(new Vector2(-100, 120), camera.Position);
        }

        [TestMethod]
        public void Get_And_Set_Camera_Viewport()
        {
            var camera = new Camera();

            camera.Viewport = new SizeF(1024, 768);
            Assert.AreEqual(new SizeF(1024, 768), camera.Viewport);

            camera.Viewport = SizeF.Empty;
            Assert.AreEqual(SizeF.Empty, camera.Viewport);
        }

        [TestMethod]
        public void Get_And_Set_Camera_Position()
        {
            var camera = new Camera();

            camera.Position = new Vector2(20, 50);
            Assert.AreEqual(new Vector2(20, 50), camera.Position);

            camera.Position = Vector2.Zero;
            Assert.AreEqual(Vector2.Zero, camera.Position);
        }
        
        [TestMethod]
        public void Convert_Camera_Screen_Space_To_World_Space()
        {
            var camera = new Camera(new SizeF(60, 32), new Vector2(100, 80));

            // Check that the sceen space top left and bottom right values are correct.
            var worldViewRect = camera.ScreenToWorld(new RectF(0, 0, camera.Viewport.Width, camera.Viewport.Height));

            Assert.AreEqual(new Vector2(70, 64), worldViewRect.TopLeft);
            Assert.AreEqual(new Vector2(130, 96), worldViewRect.BottomRight);

            // Check that vector conversion works as expected
            Assert.AreEqual(new Vector2(100, 80), worldViewRect.Center);
        }

        [TestMethod]
        public void Convert_World_Space_To_Camera_Space()
        {
            // Inverse of Convert_Camera_Screen_Space_To_World_Space parameters.
            var camera = new Camera(new SizeF(60, 32), new Vector2(100, 80));

            // Convert world rect to screen space.
            var screenRect = camera.WorldToScreen(new RectF(left: 70, top: 64, width: 60, height: 32));

            Assert.AreEqual(new Vector2(0, 0), screenRect.TopLeft);
            Assert.AreEqual(new Vector2(60, 32), screenRect.BottomRight);

            // Check that vector conversion works as expected
            Assert.AreEqual(new Vector2(30, 16), screenRect.Center);
        }
    }
}
