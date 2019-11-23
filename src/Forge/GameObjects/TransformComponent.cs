﻿/*
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

namespace Forge.GameObjects
{
    /// <summary>
    ///  Hierarchical 2d transformation component. Holds the position, rotation and scale of an object relative to the
    ///  parent and the world.
    /// </summary>
    /// <remarks>
    ///  Transform uses the Forge engine inverted Y coordinate system (eg +Y faces down and -Y faces up). This affects
    ///  properties such as Left and Right since the default rotation 0 degrees faces the +X axis.
    /// </remarks>
    public class TransformComponent : Component
    {
        private Vector2 mLocalPosition = Vector2.Zero;
        private Vector2 mWorldPosition = Vector2.Zero;

        private float mLocalRotation = 0.0f;
        private float mWorldRotation = 0.0f;
        
        private Vector2 mWorldForward = new Vector2(1.0f, 0.0f);        

        /// <summary>
        ///  Get a unit vector from the transform oriented toward the -X axis.
        /// </summary>
        public Vector2 Backward { get { return -mWorldForward; } }

        /// <summary>
        ///  Get a unit vector from the transform oriented toward the +X axis.
        /// </summary>
        public Vector2 Forward { get { return mWorldForward; } }

        /// <summary>
        ///  Get a vector that is 90 degrees left of the forward vector.
        /// </summary>
        public Vector2 Left { get { return new Vector2(mWorldForward.Y, -mWorldForward.X); } }

        /// <summary>
        ///  Get a vector that is 90 degrees right of the forward vector.
        /// </summary>
        public Vector2 Right { get { return new Vector2(-mWorldForward.Y, mWorldForward.X); } }

        /// <summary>
        ///  Get or set the position of this transform relative to the parent.
        /// </summary>
        public Vector2 LocalPosition
        {
            get { return mLocalPosition; }
            set
            {
                if (!mLocalPosition.Equals(value))
                {
                    mLocalPosition = value;
                    RegenerateWorldTransform();
                }
            }
        }

        /// <summary>
        ///  Get or set the position relative to the world.
        /// </summary>
        public Vector2 WorldPosition
        {
            get { return mWorldPosition; }
            set
            {
                if (!mWorldPosition.Equals(value))
                {
                    if (Owner != null && Owner.Parent != null)
                    {
                        LocalPosition = value - Owner.Parent.Transform.WorldPosition;
                    }
                    else
                    {
                        LocalPosition = value;
                    }
                }
            }
        }

        /// <summary>
        ///  Get or set the rotation of this transform relative to the parent.
        /// </summary>
        public float LocalRotation
        {
            get { return mLocalRotation; }
            set
            {
                if (mLocalRotation != value)
                {
                    mLocalRotation = value;
                    RegenerateWorldTransform();
                }
            }
        }

        /// <summary>
        ///  Get or set the world rotation in radians.
        ///  (Relative rotation not supported yet!!).
        /// </summary>
        public float WorldRotation
        {
            get { return mWorldRotation; }
            set
            {
                if (!mWorldRotation.Equals(value))
                {
                    if (Owner != null && Owner.Parent != null)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        LocalRotation = value;
                    }
                }
            }
        }

        /// <summary>
        ///  Transforms the provided rotation vector from local space into world space.
        /// </summary>
        /// <param name="direction">Local space direction.</param>
        /// <returns>World space direction vector</returns>
        public Vector2 TransformDirection( Vector2 direction )
        {
            throw new NotImplementedException();
        }

        public Vector2 InverseTransformDirection(Vector2 direction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Transforms the provided position vector from local space into world space.
        /// </summary>
        /// <param name="position">Position vector.</param>
        /// <returns>World space position vector.</returns>
        public Vector2 TransformPosition( Vector2 position )
        {
            throw new NotImplementedException();
        }

        public Vector2 InverseTransformPosition(Vector2 position)
        {
            throw new NotImplementedException();
        }

        // Rotate transform to make forward point at position.
        public void LookAt(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public void RotateAround(Vector2 position, float angleInRadians)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Called when game object owner's parent is changed.
        /// </summary>
        /// <param name="newParent">The new parent.</param>
        /// <param name="oldParent">The old parent.</param>
        internal void OnParentChanged(GameObject newParent, GameObject oldParent)
        {
            RegenerateWorldTransform();
        }

        /// <summary>
        ///  Method will recaculate world position, rotation and scale after a local position, rotation or scale change
        ///  has happened.
        /// </summary>
        private void RegenerateWorldTransform()
        {
            if (Owner == null)
            {
                mWorldPosition = mLocalPosition;
                mWorldRotation = MathHelper.NormalizeAngleTwoPi(mLocalRotation);
            }
            else
            {
                var parentPosition = (Owner.Parent == null ? Vector2.Zero : Owner.Parent.Transform.WorldPosition);
                var parentRotation = (Owner.Parent == null ? 0.0f : Owner.Parent.Transform.WorldRotation);

                RecalculateChildTransforms(parentPosition, parentRotation);
            }
        }

        /// <summary>
        ///  Recursively invalidate and recalculate transforms of children.
        /// </summary>
        private void RecalculateChildTransforms(Vector2 parentPosition, float parentRotation)
        {
            // Recreate world transformations.
            mWorldPosition = parentPosition + MathHelper.Rotate(mLocalPosition, parentRotation);
            mWorldRotation = MathHelper.NormalizeAngleTwoPi(parentRotation + mLocalRotation);
            mWorldForward = MathHelper.Rotate(new Vector2(1, 0), mWorldRotation);    // TODO: Do this faster.

            // Propogate transform to children.
            var currentChild = Owner.FirstChild;

            while (currentChild != null)
            {
                currentChild.Transform.RecalculateChildTransforms(mWorldPosition, mWorldRotation);
                currentChild = currentChild.NextSibling;
            }
        }

        /// <summary>
        ///  Get human readable summary of transform.
        /// </summary>
        public override string ToString()
        {
            var worldRotationDegrees = MathHelper.RadianToDegree(mWorldRotation);
            var localRotationDegrees = MathHelper.RadianToDegree(mLocalRotation); 

            return string.Format(
                "World p = {0}, r = {1} (Local p = {2}, r = {3})",
                mWorldPosition,
                worldRotationDegrees,
                mLocalPosition,
                localRotationDegrees);
        }
    }
}
