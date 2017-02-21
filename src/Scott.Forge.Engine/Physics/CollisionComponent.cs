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
        // TODO: Make these positions are center based. and document them.
        public Vector2 LastPosition { get; set; }
        public Vector2 StartingPosition { get; set; }
        public Vector2 DesiredPosition { get; set; }
        public Vector2 ActualPosition { get; set; }

        public BoundingArea Bounds { get; set; }

        /// <summary>
        ///  Offset from game object top left corner for collision bound.
        /// </summary>
        /// <remarks>
        ///  TODO: Stop using this, and use a center based rectangle instead.
        /// </remarks>
        public Vector2 Offset { get; set; }

        // TODO: Switch to an event system instead.
        public bool CollisionThisFrame { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CollisionComponent()
        {
        }
    }
}
