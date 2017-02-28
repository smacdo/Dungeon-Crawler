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
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Physics
{
    /// <summary>
    ///  Tracks object collisions.
    /// </summary>
    public class CollisionComponent : Component
    {
        public delegate void CollisionCallback(IGameObject other);

        /// <summary>
        ///  Get or set the desired position for the game object.
        /// </summary>
        public Vector2 DesiredPosition { get; internal set; }

        /// <summary>
        ///  Get or set the actual positon for the game object.
        /// </summary>
//        public Vector2 WorldPosition { get; internal set; }

        /// <summary>
        ///  Get or set the size of the collision bounding rect.
        /// </summary>
        public SizeF Size { get; set; }

        /// <summary>
        ///  Get or set the offset from the world position center.
        /// </summary>
        public Vector2 CenterOffset { get; set; }

        /// <summary>
        ///  Get or set the collision bounding box (in world space) for the game object.
        /// </summary>
        public BoundingRect WorldBounds
        {
            get
            {
                var worldPosition = (Owner != null ? Owner.Transform.WorldPosition : Vector2.Zero);
                return new BoundingRect(worldPosition + CenterOffset, Size);
            }
        }

        /// <summary>
        ///  Add or remove a callback that is triggered when this component collides with another component.
        /// </summary>
        public event CollisionCallback OnCollision;

        /// <summary>
        ///  Add or remove a callback that is triggered when this component collides with the level boundaries and
        ///  must be moved back into a valid location.
        /// </summary>
        public event CollisionCallback OnLevelBoundsCollision;

        /// <summary>
        ///  Raise a collision event with another game object.
        /// </summary>
        /// <param name="other"></param>
        internal void RaiseOnCollisionEvent(IGameObject other)
        {
            if (OnCollision != null)
            {
                OnCollision(other);
            }
        }

        /// <summary>
        ///  Raise a level boundary collision event.
        /// </summary>
        internal void RaiseOnLevelBoundsCollision()
        {
            if (OnLevelBoundsCollision != null)
            {
                OnLevelBoundsCollision(null);
            }
        }
    }
}
