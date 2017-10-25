/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Sprites;
using Scott.Forge.Tests.TestMocks;
using Scott.Forge.GameObjects;
using Scott.Forge.Settings;

namespace Scott.Forge.Tests.Sprites
{
    [TestClass]
    public class SpriteComponentProcessorTests
    {
        private const float AnimationFrameSeconds = 0.1f;

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
                        AnimationFrameSeconds,
                        new SpriteFrame[,]
                        {
                            {
                                new SpriteFrame()
                                {
                                    AtlasPosition = new Vector2(10, 15),
                                    Events = new AnimationEvent[]
                                    {
                                        new AnimationEvent("Frame0", new Dictionary<string, string>()
                                        {
                                            { "arg0", "foo" },
                                            { "arg1", "bar" }
                                        }),
                                        new AnimationEvent("FirstFrame", null)
                                    }
                                },
                                new SpriteFrame()
                                {
                                    AtlasPosition = new Vector2(20, 25),
                                    Events = new AnimationEvent[]
                                    {
                                        new AnimationEvent("SecondFrame", null)
                                    }
                                },
                                new SpriteFrame()
                                {
                                    AtlasPosition = new Vector2(30, 35)
                                },
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
        public void Calling_Update_Increments_Time_On_Each_Sprite_Component()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var s1 = CreateSprite(TestSpriteDef);

            var sp = new SpriteComponentProcessor(null);
            sp.Add(s0);
            sp.Add(s1);

            s0.PlayAnimation("TestAnimation", DirectionName.South);
            s1.PlayAnimation("TestAnimation", DirectionName.South);

            sp.Update(0.1f, AnimationFrameSeconds);

            Assert.AreEqual(0.1, s0.AnimationSecondsActive, 0.001);
            Assert.AreEqual(0.1, s1.AnimationSecondsActive, 0.001);
        }

        [TestMethod]
        public void Animation_Frame_Is_Updated_When_Sufficient_Sim_Time_Has_Passed()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South);

            // Advance time but not enough to move to the next frame.
            sp.Update(0.0, AnimationFrameSeconds / 2.0);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            // Advance time enough to move to the next frame.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);
        }

        [TestMethod]
        public void Atlas_Rect_Is_Updated_When_Animtion_Frame_Changes()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South);

            // Advance time but not enough to move to the next frame.
            sp.Update(0.0, AnimationFrameSeconds / 2.0);

            Assert.AreEqual(0, s0.AnimationFrameIndex);
            Assert.AreEqual(
                TestSpriteDef.Animations["TestAnimation"].GetAtlasPosition(DirectionName.South, 0),
                s0.SpriteRects[0].TopLeft);

            // Advance time enough to move to the next frame.
            sp.Update(0.0, AnimationFrameSeconds);

            Assert.AreEqual(1, s0.AnimationFrameIndex);
            Assert.AreEqual(
                TestSpriteDef.Animations["TestAnimation"].GetAtlasPosition(DirectionName.South, 1),
                s0.SpriteRects[0].TopLeft);
        }

        [TestMethod]
        public void Frames_Will_Be_Skipped_If_Update_Time_Is_Larger_Than_Frame_Time()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South);

            sp.Update(0.0, AnimationFrameSeconds * 2.0);
            Assert.AreEqual(2, s0.AnimationFrameIndex);
        }

        [TestMethod]
        public void Stop_And_Reset_Animation_Behaves_Correctly()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.StopAndReset);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += (o, e) =>
            {
                Assert.AreEqual("TestAnimation", e.AnimationName);
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsTrue(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(0, s0.AnimationFrameIndex);           
        }

        [TestMethod]
        public void Stop_And_Freeze_Animation_Behaves_Correctly()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.Stop);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += (o, e) =>
            {
                Assert.AreEqual("TestAnimation", e.AnimationName);
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsTrue(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);
        }

        [TestMethod]
        public void Loop_Animation_Behaves_Correctly()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.Loop);

            // Install delegate to get notification that animation completion.
            bool animationComplete = false;

            s0.AnimationCompleted += (o, e) =>
            {
                Assert.AreEqual("TestAnimation", e.AnimationName);
                animationComplete = true;
            };

            // Advance animation step by step until ending is hit.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // This call should finish the animation.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Reset is animation is true and update enough times again to hit the end.
            animationComplete = false;

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Second time end is hit and should reset.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(1, s0.AnimationFrameIndex);

            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(2, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);

            // Continue updating to make sure animation does not advance.
            sp.Update(0.0, AnimationFrameSeconds);
            Assert.AreEqual(0, s0.AnimationFrameIndex);

            Assert.IsFalse(animationComplete);
        }

        [TestMethod]
        public void Events_Are_Fired_When_Animation_Frame_Is_Shown()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);

            // Callback that adds events to the list. Makes sure to clear before advancing sim.
            var events = new List<AnimationEvent>();

            s0.AnimationEventFired += (o, e) =>
            {
                events.Add(e);
            };

            // Start animation playback.
            var ts = TimeSpan.FromSeconds(0.0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.Loop);

            // Advance simulation just enough to start animation (Frame 0).
            ts.Add(TimeSpan.FromSeconds(0.0001));
            sp.Update(ts.TotalSeconds, 0.0001);

            Assert.AreEqual(2, events.Count);

            Assert.AreEqual(1, events.Where((e) => e.Name == "FirstFrame").Count());
            Assert.AreEqual(1, events.Where((e) => e.Name == "Frame0").Count());
            Assert.AreEqual("foo", events.Where((e) => e.Name == "Frame0").First().GetArg("arg0"));
            Assert.AreEqual("bar", events.Where((e) => e.Name == "Frame0").First().GetArg("arg1"));

            events.Clear();

            // Advance simulation a tiny bit (not enough to move to the next frame) to make sure no events are added.
            ts.Add(TimeSpan.FromSeconds(0.0001));
            sp.Update(ts.TotalSeconds, 0.0001);

            Assert.AreEqual(0, events.Count);

            // Advance to the next frame.
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds);

            Assert.AreEqual(1, events.Count);

            Assert.AreEqual(1, events.Where((e) => e.Name == "SecondFrame").Count());
            Assert.AreEqual(0, events.Where((e) => e.Name == "SecondFrame").First().ArgCount);

            events.Clear();

            // Advance to the third frame. There should be no events.
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds);

            Assert.AreEqual(0, events.Count);

            // Advance through the reverse loop and to the 2nd frame.
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds);

            ts.Add(TimeSpan.FromSeconds(0.0001));
            sp.Update(ts.TotalSeconds, 0.0001);

            Assert.AreEqual(2, events.Count);

            Assert.AreEqual(1, events.Where((e) => e.Name == "FirstFrame").Count());
            Assert.AreEqual(1, events.Where((e) => e.Name == "Frame0").Count());
            Assert.AreEqual("foo", events.Where((e) => e.Name == "Frame0").First().GetArg("arg0"));
            Assert.AreEqual("bar", events.Where((e) => e.Name == "Frame0").First().GetArg("arg1"));
        }

        [TestMethod]
        public void Events_Are_Fired_Even_If_Frame_Is_Skipped()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);

            // Callback that adds events to the list. Makes sure to clear before advancing sim.
            var events = new List<AnimationEvent>();

            s0.AnimationEventFired += (o, e) =>
            {
                events.Add(e);
            };

            // Start animation playback.
            var ts = TimeSpan.FromSeconds(0.0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.Loop);

            // Advance 4 frames worth of time causing 3 frames to be skipped and the 4th frame to display.
            // (The fourth frame is frame 0 from the loop).
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds * 3 + 0.0001));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds * 3 + 0.0001);

            Assert.AreEqual(5, events.Count);

            Assert.AreEqual(2, events.Where((e) => e.Name == "FirstFrame").Count());
            Assert.AreEqual(2, events.Where((e) => e.Name == "Frame0").Count());
            Assert.IsTrue(events.Where((e) => e.Name == "Frame0").All((e) => e.GetArg("arg0") == "foo"));
            Assert.IsTrue(events.Where((e) => e.Name == "Frame0").All((e) => e.GetArg("arg1") == "bar"));
            Assert.AreEqual(1, events.Where((e) => e.Name == "SecondFrame").Count());
            Assert.IsTrue(events.Where((e) => e.Name == "SecondFrame").All((e) => e.ArgCount == 0));
        }

        [TestMethod]
        public void Stop_Animation_Action_Should_Not_Fire_Events_When_Stopping()
        {
            var s0 = CreateSprite(TestSpriteDef);
            var sp = new SpriteComponentProcessor(null);

            sp.Add(s0);

            // Callback that adds events to the list. Makes sure to clear before advancing sim.
            var events = new List<AnimationEvent>();

            s0.AnimationEventFired += (o, e) =>
            {
                events.Add(e);
            };

            // Start animation playback.
            var ts = TimeSpan.FromSeconds(0.0);
            s0.PlayAnimation("TestAnimation", DirectionName.South, AnimationEndingAction.StopAndReset);

            // Advance 3 frames worth of time causing 2 frames to be skipped and the 3rd frame to display.
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds * 2 + 0.0001));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds * 2 + 0.0001);

            Assert.AreEqual(3, events.Count);
            events.Clear();

            // Advance one frame which is enough to trigger the stop action. Verify no new events are fired.
            ts.Add(TimeSpan.FromSeconds(AnimationFrameSeconds));
            sp.Update(ts.TotalSeconds, AnimationFrameSeconds);

            Assert.AreEqual(0, events.Count);
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
