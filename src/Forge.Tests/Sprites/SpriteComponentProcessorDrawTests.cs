using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Sprites;
using Forge.Tests.TestMocks;
using Forge.GameObjects;
using Forge.Settings;
using Forge.Graphics;
using Forge;

namespace Scott.Tests.Sprites
{
    [TestClass]
    public class SpriteComponentProcessorDrawTests
    {
        /// <summary>
        ///  Test that the sprite processor will render all sprites that are attached and active.
        /// </summary>
        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void CallingDrawWillDrawAllAttachedAndEnabledSprites()
        {
            var s0 = CreateSprite(new RectF(left: 0, top: 1, width: 3, height: 4), Vector2.Zero);
            var s1 = CreateSprite(new RectF(left: 10, top: 11, width: 13, height: 14), Vector2.Zero);
            var s_disabled = CreateSprite(new RectF(left: 20, top: 21, width: 23, height: 24), Vector2.Zero);
            s_disabled.Active = false;

            var sp = new SpriteComponentProcessor(null);
            sp.Add(s0);
            sp.Add(s1);
            sp.Add(s_disabled);

            // Issue draw call, make sure only appropritate sprites were drawn. Check which sprites are drawn by
            // comparing their atlas rects (Since unit tests do not create texture atlases which would be easier).
            var r = new TestableGameRenderer();
            sp.Draw(r, new Camera(), 0.0f, 0.0f);

            Assert.AreEqual(2, r.Draws.Count);
            Assert.AreNotEqual(r.Draws[0].AtlasRect, r.Draws[1].AtlasRect);

            Assert.IsTrue(
                r.Draws[0].AtlasRect == s0.SpriteRects[0] || r.Draws[0].AtlasRect == s1.SpriteRects[0]);
            Assert.IsTrue(
                r.Draws[1].AtlasRect == s0.SpriteRects[0] || r.Draws[1].AtlasRect == s1.SpriteRects[0]);
        }

        private static SpriteComponent CreateSprite(RectF atlasRect, Vector2 gameObjectPosition)
        {
            var s = new SpriteComponent(new SpriteDefinition(
                "TestSprite",
                atlasRect.Size,
                atlasRect.TopLeft,
                null
            ));

            var go = new GameObject();
            go.Transform.WorldPosition = gameObjectPosition;
            s.Owner = go;

            return s;
        }

        [ClassInitialize]
        public static void SetUpEvironment(TestContext context)
        {
            Globals.Initialize(
                // TODO: Move TestableGameRenderer to constructor.
                new TestableDebugOverlay(),
                new ForgeSettings());
        }
    }
}
