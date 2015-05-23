/*
 * Copyright 2012-2014 Scott MacDonald
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

namespace Scott.Forge.Engine.Movement
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class MovementComponent : Component
    {
        private Vector2 mVelocity = Vector2.Zero;
        private Vector2 mLastVelocity = Vector2.Zero;
        private Vector2 mLastPosition = Vector2.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        public MovementComponent()
        {
            // Empty;
        }

        public Vector2 Acceleration { get; set; }

        /// <summary>
        ///  Get the current movement speed.
        /// </summary>
        public float Speed
        {
            get { return mVelocity.Length; }
        }

        /// <summary>
        ///  Get the current movement velocity.
        /// </summary>
        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        /// <summary>
        ///  Rectangle defining which part of the game object is used for movement related
        ///  collision detection.
        /// </summary>
        public RectF MoveBox { get; set; }

        /// <summary>
        ///  Is this component moving?
        /// </summary>
        public bool IsMoving
        {
            get
            {
                return (mVelocity.LengthSquared > 0.1f || Acceleration.LengthSquared > 0.1f);
            }
        }

        public bool CollisionThisFrame { get; set; }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StartedMovingThisFrame { get; set; }        // TODO: refer to internal movement data

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StoppedMovingThisFrame { get; set; }        // TODO: refer to internal movement data

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool ChangedDirectionThisFrame { get; set; }     // TODO: refer to internal movement data

        public void RequestMovement(DirectionName direction, float acceleration)
        {
            Acceleration = direction.ToVector() * acceleration;
        }

        /// <summary>
        /// Stops a queued move
        /// </summary>
        public void RequestStopMovement()
        {
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
        }
        
    }
}
