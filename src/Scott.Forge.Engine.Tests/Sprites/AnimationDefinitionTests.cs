using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Engine.Sprites;

namespace Scott.Forge.Engine.Tests.Sprites
{
    [TestClass]
    public class AnimationDefinitionTests
    {
        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void CreateSingleDirectionSpriteAnimationDefinition()
        {
            var frames = new List<List<RectF>>
            {
                new List<RectF> {
                    new RectF(1, 1, 5, 5),
                    new RectF(2, 3, 6, 7),
                    new RectF(4, 5, 8, 9)
                }
            };

            var a = new AnimationDefinition(
                "RandomName",
                42.5f,
                frames);

            Assert.AreEqual("RandomName", a.Name);
            Assert.AreEqual(42.5f, a.FrameTime);
            Assert.AreEqual(3, a.FrameCount);
            Assert.AreEqual(1, a.DirectionCount);
            Assert.AreEqual(frames[0][0], a.Directions[0, 0]);
            Assert.AreEqual(frames[0][1], a.Directions[0, 1]);
            Assert.AreEqual(frames[0][2], a.Directions[0, 2]);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void CreateQuadDirectionSpriteAnimationDefinition()
        {
            var frames = new List<List<RectF>>
            {
                new List<RectF> {
                    new RectF(1, 1, 5, 5),
                    new RectF(2, 3, 6, 7),
                },
                new List<RectF> {
                    new RectF(11, 11, 15, 15),
                    new RectF(12, 13, 16, 17),
                },
                new List<RectF> {
                    new RectF(21, 21, 25, 25),
                    new RectF(22, 23, 26, 27),
                },
                new List<RectF> {
                    new RectF(31, 31, 35, 35),
                    new RectF(32, 33, 36, 37),
                }
            };

            var a = new AnimationDefinition(
                "QuadAnimation",
                42.0f,
                frames);

            Assert.AreEqual("QuadAnimation", a.Name);
            Assert.AreEqual(42.0f, a.FrameTime);
            Assert.AreEqual(2, a.FrameCount);
            Assert.AreEqual(4, a.DirectionCount);
            Assert.AreEqual(frames[0][0], a.Directions[0, 0]);
            Assert.AreEqual(frames[0][1], a.Directions[0, 1]);
            Assert.AreEqual(frames[1][0], a.Directions[1, 0]);
            Assert.AreEqual(frames[1][1], a.Directions[1, 1]);
            Assert.AreEqual(frames[2][0], a.Directions[2, 0]);
            Assert.AreEqual(frames[2][1], a.Directions[2, 1]);
            Assert.AreEqual(frames[3][0], a.Directions[3, 0]);
            Assert.AreEqual(frames[3][1], a.Directions[3, 1]);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void GetSpriteRectFor()
        {
            var frames = new List<List<RectF>>
            {
                new List<RectF> {
                    new RectF(1, 1, 5, 5),
                    new RectF(2, 3, 6, 7),
                },
                new List<RectF> {
                    new RectF(11, 11, 15, 15),
                    new RectF(12, 13, 16, 17),
                },
                new List<RectF> {
                    new RectF(21, 21, 25, 25),
                    new RectF(22, 23, 26, 27),
                },
                new List<RectF> {
                    new RectF(31, 31, 35, 35),
                    new RectF(32, 33, 36, 37),
                }
            };

            var a = new AnimationDefinition(
                "QuadAnimation",
                42.0f,
                frames);

            Assert.AreEqual(frames[0][0], a.GetSpriteRectFor(DirectionName.North, 0));
            Assert.AreEqual(frames[0][1], a.GetSpriteRectFor(DirectionName.North, 1));
            Assert.AreEqual(frames[1][0], a.GetSpriteRectFor(DirectionName.West, 0));
            Assert.AreEqual(frames[1][1], a.GetSpriteRectFor(DirectionName.West, 1));
            Assert.AreEqual(frames[2][0], a.GetSpriteRectFor(DirectionName.South, 0));
            Assert.AreEqual(frames[2][1], a.GetSpriteRectFor(DirectionName.South, 1));
            Assert.AreEqual(frames[3][0], a.GetSpriteRectFor(DirectionName.East, 0));
            Assert.AreEqual(frames[3][1], a.GetSpriteRectFor(DirectionName.East, 1));
        }
    }
}
