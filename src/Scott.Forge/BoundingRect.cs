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
    ///  height extents. Similiar to the RectF in functionality.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("X = {X}, Y={Y}, HalfW={HalfWidth}, HalfH={HalfHeight}")]
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
                throw new ArgumentException("Half width cannot be less than zero", nameof(halfWidth));
            }

            if (halfHeight < 0.0f)
            {
                throw new ArgumentException("Half height cannot be less than zero", nameof(halfHeight));
            }

            mX = centerX;
            mY = centerY;
            mHalfWidth = halfWidth;
            mHalfHeight = halfHeight;
        }

        /// <summary>
        ///  Initializes a new instance of the BoundingRect structure that is defined by the given center position
        ///  and the provided extent size.
        /// </summary>
        /// <param name="center">Rectangle center position.</param>
        /// <param name="rectSize">Size of the width and height extents.</param>
        public BoundingRect(Vector2 center, SizeF rectSize)
            : this(center.X, center.Y, rectSize.Width, rectSize.Height)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BoundingRect structure that is defined by the top left and bottom
        ///  right corners.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="bottomRight">Bottom right corner of the new rectangle.</param>
        public BoundingRect(Vector2 topLeft, Vector2 bottomRight)
            : this(
                  centerX: topLeft.X + (bottomRight.X - topLeft.X) * 0.5f,
                  centerY: topLeft.Y + (bottomRight.Y - topLeft.Y) * 0.5f,
                  halfWidth: (bottomRight.X - topLeft.X) * 0.5f,
                  halfHeight: (bottomRight.Y - topLeft.Y) * 0.5f)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BoundingRect structure copied from another BoundingRect.
        /// </summary>
        /// <param name="rect">BoundingRect object to copy values from.</param>
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
        ///  Get or set the center X position of the rectangle.
        /// </summary>
        [DataMember(Name = "x", Order = 0, IsRequired = true)]
        public float X { get { return mX; } set { mX = value; } }

        /// <summary>
        ///  Get or set the center Y position of the rectangle.
        /// </summary>
        [DataMember(Name = "y", Order = 1, IsRequired = true)]
        public float Y { get { return mY; } set { mY = value; } }

        /// <summary>
        ///  Get or set the width extent.
        /// </summary>
        /// <remarks>
        ///  The width extent is half the width of the rectangle (it is the distance from the center to left or right
        ///  edge). It cannot be less than zero, and if the value is exactly zero the object is considered to be empty.
        /// </remarks>
        [DataMember(Name = "halfWidth", Order = 2, IsRequired = true)]
        public float HalfWidth
        {
            get { return mHalfWidth; }
            set
            {
                if (mHalfWidth < 0.0f)
                {
                    throw new ArgumentException("Half width cannot be less than zero", nameof(HalfWidth));
                }

                mHalfWidth = value;
            }
        }

        /// <summary>
        ///  Get or set the height extent.
        /// </summary>
        /// <remarks>
        ///  The height extent is half the height of the rectangle (it is the distance from the center to top or bottom
        ///  edge). It cannot be less than zero, and if the value is exactly zero the object is considered to be empty.
        /// </remarks>
        [DataMember(Name = "halfHeight", Order = 3, IsRequired = true)]
        public float HalfHeight
        {
            get { return mHalfHeight; }
            set
            {
                if (mHalfHeight < 0.0f)
                {
                    throw new ArgumentException("Half height cannot be less than zero", nameof(HalfHeight));
                }

                mHalfHeight = value;
            }
        }

        /// <summary>
        ///  Get or set the width of the rectangle.
        /// </summary>
        /// <remarks>
        ///  The width of the rectangle cannot be less than zero, and if the value is exactly zero the object is
        ///  considered to be empty.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public float Width
        {
            get { return HalfWidth * 2.0f; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Width cannot be less than zero", nameof(Width));
                }

                mHalfWidth = value * 0.5f;
            }
        }

        /// <summary>
        ///  Get or set the height of the rectangle.
        /// </summary>
        /// <remarks>
        ///  The height of the rectangle cannot be less than zero, and if the value is exactly zero the object is
        ///  considered to be empty.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public float Height
        {
            get { return HalfHeight * 2.0f; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Height cannot be less than zero", nameof(Height));
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
        ///  Get or set the maximum extent of the rectangle.
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
        ///  Check if the given point is contained inside of the rectangle.
        /// </summary>
        /// <remarks>
        ///  A point is contained if lies on or inside of the rectangle borders.
        /// </remarks>
        /// <param name="pointX">X position of the point.</param>
        /// <param name="pointY">Y position of the point.</param>
        /// <returns>True if the point is contained inside of the rectangle, false otherwise.</returns>
        public bool Contains(float pointX, float pointY)
        {
            return
                ((mX - mHalfWidth) <= pointX) &&
                ((mY - mHalfHeight) <= pointY) &&
                ((mX + mHalfWidth) >= pointX) &&
                ((mY + mHalfHeight) >= pointY);
        }

        /// <summary>
        ///  Check if the given point is contained inside of the rectangle.
        /// </summary>
        /// <remarks>
        ///  A point is contained if lies on or inside of the rectangle borders.
        /// </remarks>
        /// <param name="vector">Point to test.</param>
        /// <returns>True if the point is contained inside of the rectangle, false otherwise.</returns>       
        public bool Contains(Vector2 vector)
        {
            return Contains(vector.X, vector.Y);
        }
        
        /// <summary>
        ///  Check if the given bounding rect equals this object.
        /// </summary>
        /// <remarks>
        ///  Two bounding rects are considered equal if their center and extents match exactly.
        /// </remarks>
        /// <returns>True if the bounding rect matches, false otherwise.</returns>
        public bool Equals(BoundingRect other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (X == other.mX && Y == other.mY && Width == other.mHalfWidth && Height == other.mHalfHeight);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Check if the given bounding rect equals this object.
        /// </summary>
        /// <returns>True if the bounding rect matches, false otherwise.</returns>
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
                "x: {0}, y: {1}, halfW: {2}, halfH: {3}",
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

        /// <summary>
        ///  Equality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>True if the two bounding rects are equal, false otherwise.</returns>
        public static bool operator ==(BoundingRect left, BoundingRect right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///  Inequality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>True if the bounding rects are equal, false otherwise.</returns>
        public static bool operator !=(BoundingRect left, BoundingRect right)
        {
            return !(left == right);
        }
        
    }
}
