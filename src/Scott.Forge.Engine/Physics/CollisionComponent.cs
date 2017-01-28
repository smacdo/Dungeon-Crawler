/*
 * Copyright 2012-2015 Scott MacDonald
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
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Physics
{
    /// <summary>
    ///  Tracks object collisions.
    /// </summary>
    public class CollisionComponent : Component
    {
        private RectF mBroadPhaseBox;
        private BoundingArea mBounds;
        private Vector2 Offset;

        /// <summary>
        /// Constructor
        /// </summary>
        public CollisionComponent()
        {
        }

        public void Initialize(RectF boundingBox, Vector2 offset)
        {
            Offset = offset;

            mBroadPhaseBox = boundingBox;
            mBounds = new BoundingArea(BroadPhaseBox);

            Update();
        }

        public void Update()
        {
            var position = Owner.Transform.WorldPosition;
            mBroadPhaseBox.Position = position + Offset;
            mBounds.WorldPosition = position + Offset;
            CollisionThisFrame = false;
        }

        /// <summary>
        ///  Rectangle defining which part of the game object is used for movement related
        ///  collision detection.
        /// </summary>
        public RectF BroadPhaseBox {  get { return mBroadPhaseBox; } }

        public BoundingArea Bounds { get { return mBounds; } }
        
        public bool CollisionThisFrame { get; set; }
        

    }
}
