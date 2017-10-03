using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Sprites;
using Forge.GameObjects;
using Forge.Settings;
using Forge;
using Forge.Tests.TestMocks;

namespace Forge.Tests.Sprites
{
    [TestClass]
    public class SpriteComponentProcessorUpdateTests
    {
        private static readonly TimeSpan AnimationFrameTime = TimeSpan.FromSeconds(0.1);

        private static AnimatedSpriteDefinition TestSpriteDef = new AnimatedSpriteDefinition(
            new SpriteDefinition(
                "TestSprite",
                new SizeF(24, 16),
                new Vector2(30, 32),
                null),
            new AnimationSetDefinition(
                new List<AnimationDefinition>
                {
                    new AnimationDefinition(
                        "TestAnimation",
                        AnimationFrameTime,
                        new Vector2[,]
                        {
                            {
                                new Vector2(10, 15),
                                new Vector2(20, 25),
                                new Vector2(30, 35)
                            }
                        }
                    )
                }
            )
        );

        /// <summary>
        ///  Make sure time advances as expected one sprite components. Inactive components should not have their time
        ///  advanced.
        /// </summary>
        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void CallingUpdateIncrementsTimeOnEachSpriteComponent()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var s1 = CreateSprite(TestSpriteDef);

            var sp = new SpriteComponentProcessor(null);
            sp.Add(s0);
            sp.Add(s1);

            s0.PlayAnimation("TestAnimation");
            s1.PlayAnimation("TestAnimation");

            sp.Update(TimeSpan.FromSeconds(0.1), AnimationFrameTime);

            Assert.AreEqual(0.1, s0.AnimationTimeActive.TotalSeconds, 0.001);
            Assert.AreEqual(0.1, s1.AnimationTimeActive.TotalSeconds, 0.001);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void UpdateAdvancesAnimationFrames()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation");

            // Advance time but not enough to move to the next frame.
            sp.Update(TimeSpan.Zero, AnimationFrameTime / 2.0);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            // Advance time enough to move to the next frame.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void UpdatesAtlasRectWhenAnimationFrameChanges()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation");

            // Advance time but not enough to move to the next frame.
            sp.Update(TimeSpan.Zero, AnimationFrameTime / 2.0);

            Assert.AreEqual(0, s0.AnimationFrameIndex);
            Assert.AreEqual(
                TestSpriteDef.Animations["TestAnimation"].GetAtlasPosition(DirectionName.South, 0),
                s0.SpriteRects[0, 3].TopLeft);

            // Advance time enough to move to the next frame.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);

            Assert.AreEqual(1, s0.AnimationFrameIndex);
            Assert.AreEqual(
                TestSpriteDef.Animations["TestAnimation"].GetAtlasPosition(DirectionName.South, 1),
                s0.SpriteRects[0, 3].TopLeft);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void UpdateAdvancesMultipleFramesIfNeeded()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation");

            sp.Update(TimeSpan.Zero, AnimationFrameTime * 2.0);
            Assert.AreEqual(2, s0.AnimationFrameIndex);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void TestStopAndResetAnimationAction()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", AnimationEndingAction.StopAndReset);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += () =>
            {
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsTrue(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(0, s0.AnimationFrameIndex);           
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void TestStopAndFreezeAnimationAction()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", AnimationEndingAction.Stop);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += () =>
            {
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsTrue(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);
        }

        [TestMethod]
        [TestCategory("ForgeEngine/Sprites")]
        public void TestLoopAnimationAction()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", AnimationEndingAction.Loop);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += () =>
            {
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Reset is animation is true and update enough times again to hit the end.
            animationComplete = false;

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Second time end is hit and should reset.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(TimeSpan.Zero, AnimationFrameTime);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);
        }

        private static SpriteComponent CreateSprite(
            AnimatedSpriteDefinition animatedSprite,
            Vector2? gameObjectPosition = null)
        {
            var s = new SpriteComponent(animatedSprite);

            var go = new GameObject();
            go.Transform.WorldPosition = gameObjectPosition ?? Vector2.Zero;
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

        // TODO: Test when playing animation, can switch to another animation and everything works.
        // TODO:
        //  - Test start playing animation request made, then next update animation state is set correctly
        //    and the request was removed.
        //  - Test when animation is finished, public facing state is valid.
    }
}
