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
using System.Diagnostics;

namespace Scott.Forge
{
    /// <summary>
    /// Game component that detects, resolves and handles rotatable, rectangular collisions
    /// between game objects.
    /// 
    /// TODO: Do profiling to see if we should use a fitting rectangle as an early exit test
    /// (A circle would probably be even better)
    /// 
    /// TODO: Transition this to local space collision detection (currently in world space)
    /// TODO: Unit test
    /// TODO: Make this cleaner.
    /// TODO: Convert this to use Vector2 (custom) and move into Scott.Forge assembly.
    /// 
    /// HUGE HELP: http://www.metanetsoftware.com/technique/tutorialA.html
    /// 
    /// Excellent description of polygon collision + Response with C# code.
    /// http://www.codeproject.com/Articles/15573/D-Polygon-Collision-Detection
    /// 
    /// TESTS NEEDED:
    ///   Rotation
    ///   LocalOffset
    /// </summary>
    public class BoundingArea
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float Rotation { get; private set; }
        public Vector2 WorldPosition { get; set; }

        public Vector2 Size { get { return new Vector2( Width, Height ); } }

        public Vector2 UpperLeft { get { return WorldPosition + LocalUpperLeft; } }
        public Vector2 UpperRight { get { return WorldPosition + LocalUpperRight; } }
        public Vector2 LowerLeft { get { return WorldPosition + LocalLowerLeft; } }
        public Vector2 LowerRight { get { return WorldPosition + LocalLowerRight; } }

        /// <summary>
        /// The rotated rectangle's upper left vertex. This is the original upper left vertex from
        /// the unrotated rectangle, so this value may or may not actually be the upper left most
        /// point.
        /// </summary>
        public Vector2 LocalUpperLeft { get; private set; }

        /// <summary>
        /// The rotated rectangle's upper right vertex. This is the original upper right vertex from
        /// the unrotated rectangle, so this value may or may not actually be the upper right most
        /// point.
        /// </summary>
        public Vector2 LocalUpperRight { get; private set; }

        /// <summary>
        /// The rotated rectangle's lower left vertex. This is the original lower left vertex from
        /// the unrotated rectangle, so this value may or may not actually be the lower left most
        /// point.
        /// </summary>
        public Vector2 LocalLowerLeft { get; private set; }

        /// <summary>
        /// The rotated rectangle's lower right vertex. This is the original lower right vertex from
        /// the unrotated rectangle, so this value may or may not actually be the lower right most
        /// point.
        /// </summary>
        public Vector2 LocalLowerRight { get; private set; }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="dimensions">Bounding box dimensions</param>
        public BoundingArea(RectF box)
            : this(box.TopLeft, new Vector2(box.Width, box.Height))
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="dimensions">Bounding box dimensions</param>
        public BoundingArea( Vector2 topLeft, Vector2 dimensions )
            : this( topLeft, dimensions, Vector2.Zero )
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="dimensions">Original dimensions of bounding box</param>
        public BoundingArea( Vector2 topLeft, Vector2 dimensions, Vector2 offset )
        {
            Width         = dimensions.X;
            Height        = dimensions.Y;
            Rotation      = 0.0f;
            WorldPosition = topLeft + offset;

            RecalculateCachedCorners( Rotation, dimensions / 2.0f );
        }

        /// <summary>
        ///  Moves the bounding box.
        /// </summary>
        /// <param name="delta">Distance to move the bounding box.</param>
        public void Move( Vector2 delta )
        {
            WorldPosition += delta;
        }

        /// <summary>
        /// Check if the bounding rectangle intersects this bounding rectangle.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(BoundingArea other, ref Vector2 minimumTranslationVector)
        {
            // Generate the potential seperating axis vectors between our bounding rect
            // and the provided rect. We avoid the use of arrays here so we can avoid
            // garbage collection
            Vector2 v0 = LocalUpperRight - LocalUpperLeft + WorldPosition;
            Vector2 v1 = LocalUpperRight - LocalLowerRight + WorldPosition;
            Vector2 v2 = other.LocalUpperLeft - other.LocalLowerLeft + WorldPosition;
            Vector2 v3 = other.LocalUpperLeft - other.LocalUpperRight + WorldPosition;

            // Test for collision by seeing if the interval distance is less than zero. The sensitivity of the test can
            // be tweaked by changing the < 0.0f to <= 0.0f. A symbol less than seems to allow for a small amount of
            // overlap, while <= appears to trigger as soon as the lines intersect. Further testing required!
            bool collides = 
                IsAxisCollision(other, v0) && IsAxisCollision(other, v1) &&
                IsAxisCollision(other, v2) && IsAxisCollision(other, v3);

            if (collides)
            {
                minimumTranslationVector = GetMinimumTranslationVector(this, other);
            }

            return collides;
        }

        private bool IsAxisCollision(BoundingArea otherRect, Vector2 axis)
        {
            // Project the four corners of the bounding box we are checking onto the axis,
            // and get a scalar value of that projection for comparison.
            Vector2 axisNormalized = Vector2.Normalize( axis );

            float a0 = Vector2.Dot( otherRect.LocalUpperLeft, axisNormalized );
            float a1 = Vector2.Dot( otherRect.LocalUpperRight, axisNormalized );
            float a2 = Vector2.Dot( otherRect.LocalLowerLeft, axisNormalized );
            float a3 = Vector2.Dot( otherRect.LocalLowerRight, axisNormalized );

            // Project the four corners of our bounding rect onto the requested axis, and
            // get scalar values for those projections
            float b0 = Vector2.Dot( LocalUpperLeft, axisNormalized );
            float b1 = Vector2.Dot( LocalUpperRight, axisNormalized );
            float b2 = Vector2.Dot( LocalLowerLeft, axisNormalized );
            float b3 = Vector2.Dot( LocalLowerRight, axisNormalized );

            // Get the maximum and minimum scalar values for each of the rectangles
            float aMin = Math.Min( Math.Min( a0, a1 ), Math.Min( a2, a3 ) );
            float aMax = Math.Max( Math.Min( a0, a1 ), Math.Max( a2, a3 ) );
            float bMin = Math.Min( Math.Min( b0, b1 ), Math.Min( b2, b3 ) );
            float bMax = Math.Max( Math.Min( b0, b1 ), Math.Max( b2, b3 ) );

            // Test if there is an overlap between the minimum of a and the maximum values of the two rectangles.
            return (bMin <= aMax && bMax >= aMax) || (aMin <= bMax && aMax >= bMax);
        }

        /// <summary>
        /// Rotates the bounding box around our pivot axis, and then stores the rotated
        /// bounding points in Top/Left/Bottom/Right. This must be called anytime mRotation
        /// is changed!
        /// </summary>
        /// <param name="radians">Amount to rotate the rectangular bounding box</param>
        /// <param name="pivot">Position (in world coordintes) of the rotational pivot point</param>
        private void RecalculateCachedCorners( float radians, Vector2 pivot )
        {
            Rotation = MathHelper.Wrap(radians, 0f, (float)Math.PI * 2.0f);     // TODO: Verify this is safe way to wrap angle?

            // Find unrotated vertex points
            /*Vector2 oUpperLeft  = new Vector2( WorldPosition.X,         WorldPosition.Y );
            Vector2 oUpperRight = new Vector2( WorldPosition.X + Width, WorldPosition.Y );
            Vector2 oLowerLeft  = new Vector2( WorldPosition.X,         WorldPosition.Y + Height );
            Vector2 oLowerRight = new Vector2( WorldPosition.X + Width, WorldPosition.Y + Height )*/

            Vector2 oUpperLeft  = new Vector2(0, 0);
            Vector2 oUpperRight = new Vector2(Width, 0);
            Vector2 oLowerLeft  = new Vector2(0, Height);
            Vector2 oLowerRight = new Vector2(Width, Height);

            // Rotate and calculate our new rotated vertex points
            LocalUpperLeft  = RotatePoint( oUpperLeft, pivot, radians );
            LocalUpperRight = RotatePoint( oUpperRight, pivot, radians );
            LocalLowerLeft  = RotatePoint( oLowerLeft, pivot, radians );
            LocalLowerRight = RotatePoint( oLowerRight, pivot, radians );
        }

        /// <summary>
        /// Rotates a vector around a pivot point
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="origin">Pivot to rotate around</param>
        /// <param name="amount">Amount of rotation to apply</param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotatePoint( Vector2 vector, Vector2 origin, float amount )
        {
            if ( amount == 0 )
            {
                return vector;
            }
            else
            {
                return new Vector2( (float) ( origin.X + ( vector.X - origin.X ) * Math.Cos( amount ) - ( vector.Y - origin.Y ) * Math.Sin( amount ) ),
                                    (float) ( origin.Y + ( vector.Y - origin.Y ) * Math.Cos( amount ) + ( vector.X - origin.X ) * Math.Sin( amount ) ) );
            }
        }

        // NOTE: THIS ONLY WORKS FOR AABB!! Once we resume rotating the bounding area we're kinda screwed and need to
        // do the translation vector correctly.
        public static Vector2 GetMinimumTranslationVector(BoundingArea a, BoundingArea b)
        {
            Vector2 result = Vector2.Zero;
            float left = 0, right = 0, top = 0, bottom = 0;

            // Axis
            left = b.UpperLeft.X - a.UpperRight.X;
            right = b.UpperRight.X - a.UpperLeft.X;
            bottom = b.UpperLeft.Y - a.LowerLeft.Y;
            top = b.LowerLeft.Y - a.UpperLeft.Y;


            if (Math.Abs(left) > right)
            {
                result.X = right;
            }
            else
            {
                result.X = left;
            }

            if (Math.Abs(bottom) > top)
            {
                result.Y = top;
            }
            else
            {
                result.Y = bottom;
            }

            if (Math.Abs(result.X) < Math.Abs(result.Y))
            {
                result.Y = 0;
            }
            else
            {
                result.X = 0;
            }

            return result;
        }
    }
}
