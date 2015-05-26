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
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: Convert this into Transform and stick it onto GameObjects rather than keep it as a component.
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

        // Make sure rotation and direction match up by setting direction to the default direction.
        public const DirectionName DefaultDirection = DirectionName.South;

        // Local position.
        private Vector2 mPosition;
        private Vector2 mWorldPosition;

        // Local forward vector.
        private Vector2 mForward;

        // Local right vector.
        private Vector2 mRight;

        // Local rotation.
        private float mRotation;
        private float mWorldRotation;

        // Local scale.
        private Vector2 mScale = Vector2.One;

        // Local direction.
        private DirectionName mDirection = DefaultDirection;

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
        ///  Local position, relative to parent transform.
        /// </summary>
        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; mWorldPosition = value; }
        }

        /// <summary>
        ///  Local forward vector.
        /// </summary>
        public Vector2 Forward { get { return mForward; } }

        /// <summary>
        ///  Local backward vector.
        /// </summary>
        public Vector2 Backward { get { return -mForward; } }

        /// <summary>
        ///  Local right vector.
        /// </summary>
        public Vector2 Right { get { return mRight; } }

        /// <summary>
        ///  Local left vector.
        /// </summary>
        public Vector2 Left { get { return -mRight; } }

        /// <summary>
        ///  Local rotation, relative to parent transform.
        /// </summary>
        public float Rotation { get { return mRotation; } }

        /// <summary>
        ///  Local scale, relative to parent transform.
        /// </summary>
        public Vector2 Scale { get { return mScale; } }

        /// <summary>
        ///  Local direction.
        /// </summary>
        public DirectionName Direction
        {
            get
            {
                return mDirection;
            }
            set
            {
                mDirection = value;
                mRotation = value.ToRotationAngle();
                mForward = value.ToVector();
                mRight = value.ToRightVector();

                mWorldRotation = mRotation;
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
            Vector2 rPos = mPosition + MathHelper.Rotate( position - mPosition, mWorldRotation );
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
