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
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Scott.Forge.Spatial
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
    ///   
    /// TODO: Add code that when deserialized the cached corners are recalculated. (Data contract and XML).
    /// </summary>
    [DataContract]
    public class BoundingArea
    {
        // TODO: Add private fields, then make pulbic properties set-able (and recalculate) + unit tests.

        /// <summary>
        ///  Get or set the bounding area width.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Width", Order = 0, IsRequired = true)]
        public float Width { get; private set; }

        /// <summary>
        ///  Get or set the bounding area height.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Height", Order = 1, IsRequired = true)]
        public float Height { get; private set; }

        /// <summary>
        ///  Get or set the bounding area rotation in radians.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Rotation", Order = 2, IsRequired = false)]
        public float Rotation { get; private set; }

        /// <summary>
        ///  Get or set the bounding area top left position in world space.
        ///  
        ///  TODO: Strongly consider getting rid of this (speed up performance?) and make callers do the transform.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Position", Order = 3, IsRequired = false)]
        public Vector2 WorldPosition { get; set; }

        /// <summary>
        ///  Get or set the rotational pivot in local space.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Pivot", Order = 4, IsRequired = false)]
        public Vector2 Pivot { get; set; }

        /// <summary>
        ///  Get if the bounding area rectangle is empty.
        /// </summary>
        /// <remarks>
        ///  A bounding area that has a width or height of zero is considered empty.
        /// </remarks>
        public bool IsEmpty { get { return Width == 0.0f || Height == 0.0f; } }

        /// <summary>
        ///  Get the width and height of the bounding area rectangle.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "Size", Order = 4, IsRequired = true)]
        public SizeF Size { get { return new SizeF( Width, Height ); } }

        public Vector2 UpperLeft { get { return WorldPosition + LocalUpperLeft; } }
        public Vector2 UpperRight { get { return WorldPosition + LocalUpperRight; } }
        public Vector2 LowerLeft { get { return WorldPosition + LocalLowerLeft; } }
        public Vector2 LowerRight { get { return WorldPosition + LocalLowerRight; } }
        public Vector2 AxisAlignedMinPoint { get { return WorldPosition + LocalAxisAlignedMinPoint; } }
        public Vector2 AxisAlignedMaxPoint {  get { return WorldPosition + LocalAxisAlignedMaxPoint; } }

        public RectF AABB
        {
            get { return new RectF(AxisAlignedMinPoint, AxisAlignedMaxPoint); }
        }

        /// <summary>
        ///  Get the minimum point of an axis aligned bounding rectangle that encloses the bounding area.
        /// </summary>
        public Vector2 LocalAxisAlignedMinPoint { get; private set; }

        /// <summary>
        ///  Get the maximum point of an axis aligned bounding rectangle that encloses the bounding area.
        /// </summary>
        public Vector2 LocalAxisAlignedMaxPoint { get; private set; }

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
        ///  Default constructor.
        /// </summary>
        public BoundingArea()
            : this(new Vector2(0.0f, 0.0f), SizeF.Empty, 0.0f)
        {
        }

        /// <summary>
        /// Bounding box constructor.
        /// </summary>
        /// <param name="box">Rectangle defining the bounding area in world space.</param>
        /// <param name="rotation">Angle of rotation in radians.</param>
        public BoundingArea(RectF box, float rotation = 0.0f)
            : this(box.TopLeft, box.Size, rotation)
        {
        }

        /// <summary>
        /// Bounding box constructor.
        /// </summary>
        /// <param name="topLeft">Top left corner of bounding area in world space.</param>
        /// <param name="size">Bounding area width and height.</param>
        /// <param name="rotation">Angle of rotation in radians.</param>
        public BoundingArea(Vector2 topLeft, SizeF size, float rotation = 0.0f)
            : this(topLeft, size, rotation, Vector2.Zero)
        {
        }

        /// <summary>
        /// Bounding box constructor
        /// </summary>
        /// <param name="topLeft">Top left corner of bounding area in world space.</param>
        /// <param name="size">Bounding area width and height.</param>
        /// <param name="rotation">Angle of rotation in radians.</param>
        /// <param name="pivot">Location of rotation pivot in local space.</param>
        public BoundingArea(
            Vector2 topLeft,
            SizeF size,
            float rotation,
            Vector2 pivot)
        {
            Width = size.Width;
            Height = size.Height;
            Rotation = rotation;
            WorldPosition = topLeft;
            Pivot = pivot;

            RecalculateCachedCorners(Rotation, Pivot);
        }

        /// <summary>
        /// Check if the bounding rectangle intersects this bounding rectangle.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(BoundingArea other)
        {
            Vector2 temp = Vector2.Zero;
            return Intersects(other, ref temp);
        }

        /// <summary>
        /// Check if the bounding rectangle intersects this bounding rectangle.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(BoundingArea other, ref Vector2 minimumTranslationVector)
        {
            // Generate the potential seperating axis vectors between this bounding rectangle and the provided bounding
            // rectangle.
            var v0 = UpperRight - UpperLeft;
            var v1 = UpperRight - LowerRight;
            var v2 = other.UpperLeft - other.LowerLeft;
            var v3 = other.UpperLeft - other.UpperRight;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherRect"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private bool IsAxisCollision(BoundingArea otherRect, Vector2 axis)
        {
            // Project the four corners of the bounding box we are checking onto the axis,
            // and get a scalar value of that projection for comparison.
            Vector2 axisNormalized = Vector2.Normalize( axis );

            float a0 = Vector2.Dot( otherRect.UpperLeft, axisNormalized );
            float a1 = Vector2.Dot( otherRect.UpperRight, axisNormalized );
            float a2 = Vector2.Dot( otherRect.LowerLeft, axisNormalized );
            float a3 = Vector2.Dot( otherRect.LowerRight, axisNormalized );

            // Project the four corners of our bounding rect onto the requested axis, and
            // get scalar values for those projections
            float b0 = Vector2.Dot( UpperLeft, axisNormalized );
            float b1 = Vector2.Dot( UpperRight, axisNormalized );
            float b2 = Vector2.Dot( LowerLeft, axisNormalized );
            float b3 = Vector2.Dot( LowerRight, axisNormalized );

            // Get the maximum and minimum scalar values for each of the rectangles
            float aMin = Math.Min( Math.Min( a0, a1 ), Math.Min( a2, a3 ) );
            float aMax = Math.Max( Math.Min( a0, a1 ), Math.Max( a2, a3 ) );
            float bMin = Math.Min( Math.Min( b0, b1 ), Math.Min( b2, b3 ) );
            float bMax = Math.Max( Math.Min( b0, b1 ), Math.Max( b2, b3 ) );

            // Test if there is an overlap between the minimum of a and the maximum values of the two rectangles.
            return (bMin < aMax && bMax > aMax) || (aMin < bMax && aMax > bMax);
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
            // Find unrotated vertex points.
            LocalUpperLeft  = new Vector2(0, 0);
            LocalUpperRight = new Vector2(Width, 0);
            LocalLowerLeft  = new Vector2(0, Height);
            LocalLowerRight = new Vector2(Width, Height);

            // Rotate bounding rect vertices.
            if (radians != 0)
            {
                var cosTheta = (float)Math.Cos(radians);
                var sinTheta = (float)Math.Sin(radians);

                LocalUpperLeft = RotatePoint(LocalUpperLeft, pivot, cosTheta, sinTheta);
                LocalUpperRight = RotatePoint(LocalUpperRight, pivot, cosTheta, sinTheta);
                LocalLowerLeft = RotatePoint(LocalLowerLeft, pivot, cosTheta, sinTheta);
                LocalLowerRight = RotatePoint(LocalLowerRight, pivot, cosTheta, sinTheta);
            }

            // Find an axis aligned bounding box that encloses the bounding area.
            var minX =
                Math.Min(LocalUpperLeft.X, Math.Min(LocalUpperRight.X, Math.Min(LocalLowerLeft.X, LocalLowerRight.X)));
            var minY =
                Math.Min(LocalUpperLeft.Y, Math.Min(LocalUpperRight.Y, Math.Min(LocalLowerLeft.Y, LocalLowerRight.Y)));
            var maxX =
                Math.Max(LocalUpperLeft.X, Math.Max(LocalUpperRight.X, Math.Max(LocalLowerLeft.X, LocalLowerRight.X)));
            var maxY =
                Math.Max(LocalUpperLeft.Y, Math.Max(LocalUpperRight.Y, Math.Max(LocalLowerLeft.Y, LocalLowerRight.Y)));

            LocalAxisAlignedMinPoint = new Vector2(minX, minY);
            LocalAxisAlignedMaxPoint = new Vector2(maxX, maxY);
        }

        /// <summary>
        ///  Rotate a point around another point.
        /// </summary>
        /// <param name="vector">Point to rotate.</param>
        /// <param name="origin">Point to rotate around.</param>
        /// <param name="amount">Cosine of rotation angle in radians.</param>
        /// <param name="sinTheta">Sin of rotation angle in radians.</param>
        /// <returns>Rotated point.</returns>
        private static Vector2 RotatePoint(Vector2 vector, Vector2 origin, float cosTheta, float sinTheta)
        {
            return new Vector2(
                cosTheta * (vector.X - origin.X) - sinTheta * (vector.Y - origin.Y) + origin.X,
                sinTheta * (vector.X - origin.X) + cosTheta * (vector.Y - origin.Y) + origin.Y);
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
