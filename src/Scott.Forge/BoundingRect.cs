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
using System.Runtime.Serialization;

namespace Scott.Forge
{
    /// <summary>
    ///  A two dimensional axis aligned bounding rectangle that is defined by a center position and its half width/
    ///  height extents. Similiar to the BoundingRect class but different [TODO: Elaborate].
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("Center = ({X}, Y={Y}) Width={Width}, Height={Height}")]
    public struct BoundingRect : IEquatable<BoundingRect>
    {
        private static readonly BoundingRect mEmpty = new BoundingRect(0.0f, 0.0f, 0.0f, 0.0f);
        private float mX;
        private float mY;
        private float mHalfWidth;
        private float mHalfHeight;

        /// <summary>
        ///  Initializes a new instance of the BoundingRect structure that is defined by the given center position
        ///  and the extents.
        /// </summary>
        /// <param name="centerX">Center X coordinate for the rectangle.</param>
        /// <param name="centerY">Center Y coordinate for the rectangle.</param>
        /// <param name="halfWidth">Width extent of the new rectangle.</param>
        /// <param name="heightHeight">Height extent of the new rectangle.</param>
        public BoundingRect(float centerX, float centerY, float halfWidth, float halfHeight)
        {
            if (halfWidth < 0.0f)
            {
                throw new ArgumentException("Width extent cannot be less than zero", "halfWidth");
            }

            if (halfHeight < 0.0f)
            {
                throw new ArgumentException("Height extent cannot be less than zero", "halfHeight");
            }

            mX = centerX;
            mY = centerY;
            mHalfWidth = halfWidth;
            mHalfHeight = halfHeight;
        }

        public BoundingRect(Vector2 center, SizeF rectSize)
            : this(center.X, center.Y, rectSize.Width, rectSize.Height)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BoundingRect structure that is defined by the given top
        ///  left and bottom right points.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="bottomRight">Bottom right corner of the new rectangle.</param>
        public BoundingRect(Vector2 topLeft, Vector2 bottomRight)
            : this(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y)
        {
        }

        public BoundingRect(BoundingRect rect)
            : this(rect.mX, rect.mY, rect.mHalfWidth, rect.mHalfHeight)
        {
        }

        /// <summary>
        ///  Get an empty rectangle.
        /// </summary>
        /// <remarks>
        ///  An empty rectangle is one anchored at (0,0) and has both the width and height set
        ///  to zero.
        /// </remarks>
        public static BoundingRect Empty { get { return Empty; } }

        /// <summary>
        ///  Get or set the left-most X position of the rectangle.
        /// </summary>
        [DataMember(Name = "CenterX", Order = 0, IsRequired = true)]
        public float X { get { return mX; } set { mX = value; } }

        /// <summary>
        ///  Get or set the top-most Y position of the rectangle.
        /// </summary>
        [DataMember(Name = "CenterY", Order = 1, IsRequired = true)]
        public float Y { get { return mY; } set { mY = value; } }

        /// <summary>
        ///  Get or set the width extent.
        /// </summary>
        [DataMember(Name = "HalfWidth", Order = 2, IsRequired = true)]
        public float HalfWidth
        {
            get { return mHalfWidth; }
            set
            {
                if (mHalfWidth < 0.0f)
                {
                    throw new ArgumentException("Half width cannot be less than zero", "HalfWidth");
                }

                mHalfWidth = value;
            }
        }

        /// <summary>
        ///  Get or set the height extent;.
        /// </summary>
        [DataMember(Name = "HalfHeight", Order = 3, IsRequired = true)]
        public float HalfHeight
        {
            get { return mHalfHeight; }
            set
            {
                if (mHalfHeight < 0.0f)
                {
                    throw new ArgumentException("Half height cannot be less than zero", "HalfHeight");
                }

                mHalfHeight = value;
            }
        }

        /// <summary>
        ///  Get or set the width of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Width
        {
            get { return HalfWidth * 2.0f; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Width cannot be less than zero", "Width");
                }

                mHalfWidth = value * 0.5f;
            }
        }

        /// <summary>
        ///  Get or set the height of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Height
        {
            get { return HalfHeight * 2.0f; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Height cannot be less than zero", "Height");
                }

                mHalfHeight = value * 0.5f;
            }
        }

        /// <summary>
        ///  Get if the rectangle is empty.
        /// </summary>
        /// <remarks>
        ///  A rectangle is considered empty when either its width or height is zero.
        /// </remarks>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return mHalfWidth == 0.0f || mHalfHeight == 0.0f; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Get the maximum extent of the rectangle.
        /// </summary>
        public Vector2 MaxPoint
        {
            get { return new Vector2(mX + mHalfWidth, mY + mHalfHeight); }
        }

        /// <summary>
        ///  Get the minimum extent of the rectangle.
        /// </summary>
        public Vector2 MinPoint
        {
            get { return new Vector2(mX - mHalfWidth, mY - mHalfHeight); }
        }

        /// <summary>
        ///  Get or set the center of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Center
        {
            get { return new Vector2(mX, mY); }
            set
            {
                mX = value.X;
                mY = value.Y;
            }
        }

        /// <summary>
        ///  Get or set the extents of the bounding rectangle.
        /// </summary>
        /// <remarks>
        ///  The extents are the half width and half height of the rectangle.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Extents
        {
            get { return new Vector2(mHalfWidth, mHalfHeight); }
            set
            {
                HalfWidth = value.X;
                HalfHeight = value.Y;
            }
        }

        /// <summary>
        ///  Get or set the extens of the rectangle.
        /// </summary>
        /// <remarks>
        ///  The extents are the half width and half height of the rectangle.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public SizeF ExtentSize
        {
            get { return new SizeF(mHalfWidth, mHalfHeight); }
            set
            {
                HalfWidth = value.Width;
                HalfHeight = value.Height;
            }
        }
        
        /// <summary>
        ///  Check if the given point is contained inside of the rectangle. A point is contained
        ///  if lies on or inside of the rectangle borders.
        /// </summary>
        /// <param name="pointX">X component of the point.</param>
        /// <param name="pointY">Y component of the point.</param>
        /// <returns>
        ///  True if the point is contained inside of the rectangle, false otherwise.
        /// </returns>
        public bool Contains(float pointX, float pointY)
        {
            return
                ((mX - mHalfWidth) <= pointX) &&
                ((mY - mHalfHeight) <= pointY) &&
                ((mX + mHalfWidth) >= pointX) &&
                ((mY + mHalfHeight) >= pointY);
        }

        /// <summary>
        ///  Check if the given point is contained inside of the rectangle. A point is contained
        ///  if lies on or inside of the rectangle borders.
        /// </summary>
        /// <param name="vector">Point to test.</param>
        /// <returns>
        ///  True if the point is contained inside of the rectangle, false otherwise.
        /// </returns>       
        public bool Contains(Vector2 vector)
        {
            return Contains(vector.X, vector.Y);
        }
        
        /// <summary>
        ///  Check if another rectangle equals this rectangle's value.
        /// </summary>
        /// <returns>True if the rectangles have the same value, false otherwise.</returns>
        public bool Equals(BoundingRect other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (X == other.X && Y == other.Y && Width == other.HalfWidth && Height == other.HalfHeight);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Check if another object equals this rectangle's value.
        /// </summary>
        /// <returns>True if the rectangles have the same value, false otherwise.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is BoundingRect)
            {
                return Equals((BoundingRect) obj);
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        ///  Get the object's string representation.
        /// </summary>
        /// <returns>Object's string representation.</returns>
        public override string ToString()
        {
            return String.Format(
                "cx: {0}, cy: {1}, hw: {2}, hh: {3}",
                X,
                Y,
                HalfWidth,
                HalfHeight);
        }

        /// <summary>
        ///  Get the object's hash code.
        /// </summary>
        /// <returns>Object's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash *= 23 + mX.GetHashCode();
                hash *= 23 + mY.GetHashCode();
                hash *= 23 + mHalfWidth.GetHashCode();
                hash *= 23 + mHalfHeight.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(BoundingRect left, BoundingRect right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoundingRect left, BoundingRect right)
        {
            return !(left == right);
        }
        
    }
}
