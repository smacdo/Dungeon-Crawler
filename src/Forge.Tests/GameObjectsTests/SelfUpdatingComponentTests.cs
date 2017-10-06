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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.GameObjects;

namespace Forge.Tests.GameObjectsTests
{
    [TestClass]
    public class SelfUpdatingComponentTests
    {
        [TestMethod]
        public void Self_Updating_Components_On_Game_Object_Get_Updated()
        {
            var gameObject = new GameObject();
            var a = gameObject.Add(new MySelfUpdatingComponent_A());
            var b = gameObject.Add(new MySelfUpdatingComponent_B());

            Assert.AreEqual(0.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, a.LastDeltaTime.TotalSeconds);
            Assert.AreEqual(0.0, b.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, b.LastDeltaTime.TotalSeconds);

            // Run an update call, see if they get updated.
            gameObject.UpdateSelf(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.2));

            Assert.AreEqual(1.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.2, a.LastDeltaTime.TotalSeconds, 0.001);
            Assert.AreEqual(1.0, b.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.2, b.LastDeltaTime.TotalSeconds, 0.001);

            // One more update call this time to recursively just to make sure.
            gameObject.UpdateRecursively(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(0.3));

            Assert.AreEqual(2.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.3, a.LastDeltaTime.TotalSeconds, 0.001);
            Assert.AreEqual(2.0, b.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.3, b.LastDeltaTime.TotalSeconds, 0.001);
        }

        [TestMethod]
        public void Removed_Components_Do_Not_Update_1()
        {
            // Test removal of head.
            var gameObject = new GameObject();

            var a = gameObject.Add(new MySelfUpdatingComponent_A());
            var b = gameObject.Add(new MySelfUpdatingComponent_B());

            gameObject.Remove<MySelfUpdatingComponent_A>();
            
            // Run an update call, see if they get updated.
            gameObject.UpdateSelf(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.2));

            Assert.AreEqual(0.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, a.LastDeltaTime.TotalSeconds, 0.001);
            Assert.AreEqual(1.0, b.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.2, b.LastDeltaTime.TotalSeconds, 0.001);
        }

        [TestMethod]
        public void Recurisve_Update_Correctly_Updates_Self_Updating_Components()
        {
            var root = new GameObject();
            var a = root.Add(new MySelfUpdatingComponent_A());

            var child_1 = new GameObject();
            child_1.Parent = root;

            var a_1 = child_1.Add(new MySelfUpdatingComponent_A());

            var child_2 = new GameObject();
            child_2.Parent = root;

            var a_2 = child_2.Add(new MySelfUpdatingComponent_A());

            var child_3 = new GameObject();
            child_3.Parent = root;

            var a_3 = child_3.Add(new MySelfUpdatingComponent_A());

            // Initialize all values are zero.
            Assert.AreEqual(0.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, a_1.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, a_2.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, a_3.LastCurrentTime.TotalSeconds);

            // Run an update call, see if they get updated.
            root.UpdateRecursively(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.2));

            Assert.AreEqual(1.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(1.0, a_1.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(1.0, a_2.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(1.0, a_3.LastCurrentTime.TotalSeconds);
        }

        [TestMethod]
        public void Removed_Components_Do_Not_Update_2()
        {
            // Test removal of second element.
            var gameObject = new GameObject();

            var a = gameObject.Add(new MySelfUpdatingComponent_A());
            var b = gameObject.Add(new MySelfUpdatingComponent_B());

            gameObject.Remove<MySelfUpdatingComponent_B>();

            // Run an update call, see if they get updated.
            gameObject.UpdateSelf(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.2));

            Assert.AreEqual(1.0, a.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.2, a.LastDeltaTime.TotalSeconds, 0.001);
            Assert.AreEqual(0.0, b.LastCurrentTime.TotalSeconds);
            Assert.AreEqual(0.0, b.LastDeltaTime.TotalSeconds, 0.001);
        }
        
        
        class MySelfUpdatingComponent_A : SelfUpdatingComponent
        {
            public TimeSpan LastCurrentTime { get; internal set; }
            public TimeSpan LastDeltaTime { get; internal set; }

            public override void OnUpdate(TimeSpan currentTime, TimeSpan deltaTime)
            {
                LastCurrentTime = currentTime;
                LastDeltaTime = deltaTime;
            }
        }

        class MySelfUpdatingComponent_B : SelfUpdatingComponent
        {
            public TimeSpan LastCurrentTime { get; internal set; }
            public TimeSpan LastDeltaTime { get; internal set; }

            public override void OnUpdate(TimeSpan currentTime, TimeSpan deltaTime)
            {
                LastCurrentTime = currentTime;
                LastDeltaTime = deltaTime;
            }
        }

        class MyNormalComponent : Component
        {
            // Nothing...
        }
    }
}
