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
using System.Text;

namespace Scott.Forge.GameObjects
{
    /// <summary>
    ///  Contains the position, rotation and direction of an object. All coordinate and rotational values are in local
    ///  space and relative to the transform's parent unless explicitly stated.
    /// </summary>
    public class TransformComponent : Component
    {
        // Default scale is one, never zero.
        public static readonly Vector2 DefaultScale = Vector2.One;

        /// <summary>
        ///  Offset from local origin.
        /// </summary>
        private Vector2 mLocalPosition = Vector2.Zero;

        /// <summary>
        ///  Offset from world origin. Calculate as parent.WorldPosition + this.LocalPosition.
        /// </summary>
        private Vector2 mWorldPosition = Vector2.Zero;

        private float mLocalRotation = 0.0f;
        private float mWorldRotation = 0.0f;

        private Vector2 mLocalScale = Vector2.One;
        private Vector2 mWorldScale = Vector2.One;

        // Cached from mWorldRotation.
        private Vector2 mWorldForward;        

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <remarks>
        ///  The transform's direction property is set to south.
        /// </remarks>
        public TransformComponent()
        {
        }

        /// <summary>
        ///  Object world position.
        /// </summary>
        public Vector2 Position
        {
            get { return mLocalPosition; }
            set { mLocalPosition = value; mWorldPosition = value; }
        }

        /// <summary>
        ///  Forward vector in world space.
        ///  TODO: This is wrong.
        /// </summary>
        public Vector2 Forward { get { return mWorldForward; } }

        /// <summary>
        ///  Backward vector in world space.
        ///  TODO: This is wrong.
        /// </summary>
        public Vector2 Backward { get { return -mWorldForward; } }

        /// <summary>
        ///  Right vector in world space.
        ///   TODO: This is wrong.
        /// </summary>
        public Vector2 Right { get { return mWorldRight; } }

        /// <summary>
        ///  Left vector in world space.
        ///   TODO: This is wrong.
        /// </summary>
        public Vector2 Left { get { return -mWorldRight; } }

        /// <summary>
        ///  Local rotation, relative to parent transform. Rotation is in radians.
        /// </summary>
        public float Rotation
        {
            get { return mWorldRotation; }
            set
            {
                // TODO: Wrap angle.
                mWorldRotation = value;
                mWorldForward = MathHelper.Rotate(0, 1, (float)Math.PI * 2.0f - value);
            }
        }

        /// <summary>
        ///  Local scale, relative to parent transform.
        /// </summary>
        public Vector2 Scale { get { return mLocalScale; } }

        /// <summary>
        ///  World direction.
        /// </summary>
        public DirectionName Direction
        {
            get
            {
                return DirectionNameHelper.FromRotationDegrees(mWorldRotation);
            }
            set
            {
                mLocalRotation = MathHelper.DegreeToRadian(value.ToRotationDegrees());
                mWorldForward = value.ToVector();
                mWorldRight = value.ToRightVector();

                mWorldRotation = mLocalRotation;
            }
        }

        /// <summary>
        ///  Move the transform in the requested distance, relative to the transform's current
        ///  rotation.
        /// </summary>
        /// <param name="distance">Distance to move.</param>
        public void Translate( Vector2 distance )
        {
            Position += distance;
        }

        /// <summary>
        ///  Transforms the provided rotation vector from local space into world space.
        /// </summary>
        /// <param name="direction">Local space direction.</param>
        /// <returns>World space direction vector</returns>
        public Vector2 TransformDirection( Vector2 direction )
        {
            return MathHelper.Rotate( direction, mWorldRotation );
        }

        /// <summary>
        ///  Transforms the provided position vector from local space into world space.
        /// </summary>
        /// <param name="position">Position vector.</param>
        /// <returns>World space position vector.</returns>
        public Vector2 TransformPosition( Vector2 position )
        {
            Vector2 rPos = mLocalPosition + MathHelper.Rotate( position - mLocalPosition, mWorldRotation );
            return mWorldPosition + rPos;
        }

        /// <summary>
        ///  Convert the Transform to a human readable string.
        /// </summary>
        /// <returns>String readable transform.</returns>
        public override string ToString()
        {
            return
                "Position <{0},{1}>, Dir: {2}, Rotation: {3}, Scale: <{4},{5}>"
                    .With( Position.X, Position.Y,
                           Direction,
                           Rotation,
                           Scale.X,
                           Scale.Y );
        }
    }
}
