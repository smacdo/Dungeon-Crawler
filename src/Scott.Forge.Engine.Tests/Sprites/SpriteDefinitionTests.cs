using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using Scott.Forge.Engine.Sprites;

namespace Scott.Forge.Engine.Tests.Sprites
{
    [TestClass]
    public class SpriteDefinitionTests
    {
        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/SpriteDefinition")]
        public void CreateStaticSpriteDefinition()
        {
            List<List<RectF>> frames = null;
            var animation = CreateAnimationDefinition(1, 1, out frames, "SingleFrameAnimation", 0.25f);


        }

        private AnimationDefinition CreateAnimationDefinition(
            int frameCount,
            int directionCount,
            out List<List<RectF>> frames,
            string name = "DefaultName",
            float frameTime = 0.50f)
        {
            frames = new List<List<RectF>>(directionCount);

            foreach (var di in Enumerable.Range(0, directionCount))
            {
                frames[di] = new List<RectF>();

                foreach (var fi in Enumerable.Range(0, frameCount))
                {
                    frames[di].Add(new RectF(
                        10 * di + fi + 0,
                        10 * di + fi + 1,
                        10 * di + fi * 2,
                        10 * di + fi * 3));
                }
            }

            return new AnimationDefinition(name, frameTime, frames);
        }
    }
}
