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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine.Movement
{
    /// <summary>
    /// Tracks movement information
    /// </summary>
    public class MovementComponent : Component
    {
        private float mVelocity = 0.0f;
        private float mLastVelocity = 0.0f;
        private DirectionName mDirection = DirectionName.South;
        private DirectionName mLastDirection = DirectionName.South;
        private Vector2 mPosition = Vector2.Zero;
        private Vector2 mLastPosition = Vector2.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        public MovementComponent()
        {
            // Empty;
        }

        /// <summary>
        ///  The speed (in units/sec) that the movement is occurring
        /// </summary>
        public float Velocity { get { return mVelocity; } set { mVelocity = value; } }

        /// <summary>
        ///  The direction that this component is moving.
        /// </summary>
        public DirectionName Direction { get { return mDirection; } set { mDirection = value; } }

        /// <summary>
        ///  Rectangle defining which part of the game object is used for movement related
        ///  collision detection.
        /// </summary>
        public RectangleF MoveBox { get; set; }

        /// <summary>
        ///  Is this component moving?
        /// </summary>
        public bool IsMoving
        {
            get
            {
                return ( Math.Abs( mVelocity ) < 0.1f );
            }
        }

        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
            }
        }

        public Vector2 LastPosition { get { return mLastPosition; } }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StartedMovingThisFrame { get; private set; }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool StoppedMovingThisFrame { get; private set; }

        /// <summary>
        ///  True if this is the update call that started the component moving.
        /// </summary>
        public bool ChangedDirectionThisFrame { get; private set; }

        /// <summary>
        /// Requests the actor to move in a specified direction for the duration of this update
        /// cycle
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        /// <param name="velocity">Speed at which to move</param>
        public void Move(DirectionName direction, float velocity)
        {
            mVelocity = velocity;
            mDirection = direction;
        }


        public void Update(Vector2 position, float velocity, DirectionName direction)
        {
            // Update our new values and store our old ones.
            mLastVelocity = mVelocity;
            mLastDirection = mDirection;
            mLastPosition = mPosition;

            mPosition = position;
            mVelocity = velocity;

            // Did we start or stop moving this frame?

            // ReSharper disable CompareOfFloatsByEqualityOperator
            StartedMovingThisFrame =
                ((mVelocity > 0.0f || mVelocity < 0.0f) && mLastVelocity == 0.0f);

            StoppedMovingThisFrame =
                (mVelocity == 0.0f && (mLastVelocity > 0.0f || mLastVelocity < 0.0f));
            // ReSharper restore CompareOfFloatsByEqualityOperator

            // How about direction? Did we change direction this frame?
            ChangedDirectionThisFrame = (mDirection != mLastDirection);


            mVelocity = 0.0f;           // this seems like a stupid hack. TODO: USe proper movement physics.
        }

        /// <summary>
        /// Stops a queued move
        /// </summary>
        public void CancelMove()
        {
            mVelocity = 0.0f;
        }

        public void ChangeDirection( DirectionName direction )
        {
            Direction = direction;
        }

        
    }
}
