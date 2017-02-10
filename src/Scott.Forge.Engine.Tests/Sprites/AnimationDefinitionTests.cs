﻿using System;
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
            var frames = new List<List<Vector2>>
            {
                new List<Vector2> {
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
            Assert.AreEqual(frames[0][0], a.Frames[0, 0]);
            Assert.AreEqual(frames[0][1], a.Frames[0, 1]);
            Assert.AreEqual(frames[0][2], a.Frames[0, 2]);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void CreateQuadDirectionSpriteAnimationDefinition()
        {
            var frames = new List<List<Vector2>>
            {
                new List<Vector2> {
                    new Vector2(1, 1),
                    new Vector2(2, 3),
                },
                new List<Vector2> {
                    new Vector2(11, 11),
                    new Vector2(12, 13),
                },
                new List<Vector2> {
                    new Vector2(21, 21),
                    new Vector2(22, 23),
                },
                new List<Vector2> {
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
            Assert.AreEqual(frames[0][0], a.Frames[0, 0]);
            Assert.AreEqual(frames[0][1], a.Frames[0, 1]);
            Assert.AreEqual(frames[1][0], a.Frames[1, 0]);
            Assert.AreEqual(frames[1][1], a.Frames[1, 1]);
            Assert.AreEqual(frames[2][0], a.Frames[2, 0]);
            Assert.AreEqual(frames[2][1], a.Frames[2, 1]);
            Assert.AreEqual(frames[3][0], a.Frames[3, 0]);
            Assert.AreEqual(frames[3][1], a.Frames[3, 1]);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites/AnimationDefinition")]
        public void GetSpriteFrame()
        {
            var frames = new List<List<Vector2>>
            {
                new List<Vector2> {
                    new Vector2(1, 1),
                    new Vector2(2, 3),
                },
                new List<Vector2> {
                    new Vector2(11, 11),
                    new Vector2(12, 13),
                },
                new List<Vector2> {
                    new Vector2(21, 21),
                    new Vector2(22, 23),
                },
                new List<Vector2> {
                    new Vector2(31, 31),
                    new Vector2(32, 33),
                }
            };

            var a = new AnimationDefinition(
                "QuadAnimation",
                42.0f,
                frames);

            Assert.AreEqual(frames[0][0], a.GetSpriteFrame(DirectionName.East, 0));
            Assert.AreEqual(frames[0][1], a.GetSpriteFrame(DirectionName.East, 1));
            Assert.AreEqual(frames[1][0], a.GetSpriteFrame(DirectionName.South, 0));
            Assert.AreEqual(frames[1][1], a.GetSpriteFrame(DirectionName.South, 1));
            Assert.AreEqual(frames[2][0], a.GetSpriteFrame(DirectionName.West, 0));
            Assert.AreEqual(frames[2][1], a.GetSpriteFrame(DirectionName.West, 1));
            Assert.AreEqual(frames[3][0], a.GetSpriteFrame(DirectionName.North, 0));
            Assert.AreEqual(frames[3][1], a.GetSpriteFrame(DirectionName.North, 1));
        }
    }
}