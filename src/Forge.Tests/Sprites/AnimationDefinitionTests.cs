using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Sprites;

namespace Forge.Tests.Sprites
{
    [TestClass]
    public class AnimationDefinitionTests
    {
        [TestMethod]
        public void Create_Single_Direction_Sprite_Animation_Definition()
        {
            var frames = new Vector2[,]
            {
                {
                    new Vector2(1, 1),
                    new Vector2(2, 3),
                    new Vector2(4, 5)
                }
            };

            var a = new AnimationDefinition(
                "RandomName",
                42.5f,
                frames);

            Assert.AreEqual("RandomName", a.Name);
            Assert.AreEqual(42.5f, a.FrameSeconds);
            Assert.AreEqual(3, a.FrameCount);
            Assert.AreEqual(frames[0, 0], a.Frames[0, 0]);
            Assert.AreEqual(frames[0, 1], a.Frames[0, 1]);
            Assert.AreEqual(frames[0, 2], a.Frames[0, 2]);
        }

        [TestMethod]
        public void Create_Quad_Direction_Sprite_Animation_Definition()
        {
            var frames = new Vector2[,]
            {
                {
                    new Vector2(1, 1),
                    new Vector2(2, 3),
                },
                {
                    new Vector2(11, 11),
                    new Vector2(12, 13),
                },
                {
                    new Vector2(21, 21),
                    new Vector2(22, 23),
                },
                {
                    new Vector2(31, 31),
                    new Vector2(32, 33),
                }
            };

            var a = new AnimationDefinition(
                "QuadAnimation",
                42.0f,
                frames);

            Assert.AreEqual("QuadAnimation", a.Name);
            Assert.AreEqual(42.0f, a.FrameSeconds);
            Assert.AreEqual(2, a.FrameCount);
            Assert.AreEqual(frames[0, 0], a.Frames[0, 0]);
            Assert.AreEqual(frames[0, 1], a.Frames[0, 1]);
            Assert.AreEqual(frames[1, 0], a.Frames[1, 0]);
            Assert.AreEqual(frames[1, 1], a.Frames[1, 1]);
            Assert.AreEqual(frames[2, 0], a.Frames[2, 0]);
            Assert.AreEqual(frames[2, 1], a.Frames[2, 1]);
            Assert.AreEqual(frames[3, 0], a.Frames[3, 0]);
            Assert.AreEqual(frames[3, 1], a.Frames[3, 1]);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void Get_Sprite_Frame_For_Different_Directions()
        {
            var frames = new Vector2[,]
            { 
                {
                    new Vector2(1, 1),
                    new Vector2(2, 3),
                },
                {
                    new Vector2(11, 11),
                    new Vector2(12, 13),
                },
                {
                    new Vector2(21, 21),
                    new Vector2(22, 23),
                },
                {
                    new Vector2(31, 31),
                    new Vector2(32, 33),
                }
            };

            var a = new AnimationDefinition(
                "QuadAnimation",
                42.0f,
                frames);

            Assert.AreEqual(frames[0, 0], a.GetAtlasPosition(DirectionName.East, 0));
            Assert.AreEqual(frames[0, 1], a.GetAtlasPosition(DirectionName.East, 1));
            Assert.AreEqual(frames[1, 0], a.GetAtlasPosition(DirectionName.South, 0));
            Assert.AreEqual(frames[1, 1], a.GetAtlasPosition(DirectionName.South, 1));
            Assert.AreEqual(frames[2, 0], a.GetAtlasPosition(DirectionName.West, 0));
            Assert.AreEqual(frames[2, 1], a.GetAtlasPosition(DirectionName.West, 1));
            Assert.AreEqual(frames[3, 0], a.GetAtlasPosition(DirectionName.North, 0));
            Assert.AreEqual(frames[3, 1], a.GetAtlasPosition(DirectionName.North, 1));
        }
    }
}
