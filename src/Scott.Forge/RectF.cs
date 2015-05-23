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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Scott.Forge
{
    /// <summary>
    ///  A two dimesional axis aligned rectangle that is defined by its upper left corner (X,Y)
    ///  and its width and height. Size and position are stored using floating point values.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("X={X}, Y={Y}, Width={Width}, Height={Height}")]
    public struct RectF : IEquatable<RectF>
    {
        private static readonly RectF EmptyRect = new RectF(0.0f, 0.0f, 0.0f, 0.0f);

        private float mX;
        private float mY;
        private float mWidth;
        private float mHeight;

        /// <summary>
        ///  Initializes a new instance of the RectF structure that is defined by the provided x,
        ///  y, width and height values.
        /// </summary>
        /// <param name="topX">Top X coordinate for the rectangle.</param>
        /// <param name="topY">Top Y coordinate for the rectangle.</param>
        /// <param name="width">Width of the new rectangle.</param>
        /// <param name="height">Height of the new rectangle.</param>
        public RectF(float topX, float topY, float width, float height)
        {
            if (width < 0.0f)
            {
                throw new ArgumentException("Width cannot be less than zero", "width");
            }

            if (height < 0.0f)
            {
                throw new ArgumentException("Height cannot be less than zero", "height");
            }

            mX = topX;
            mY = topY;
            mWidth = width;
            mHeight = height;
        }

        /// <summary>
        ///  Initializes a new instance of the Rect structure that has the specified top left
        ///  corner location and the specified width and height.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="rectSize">Size of the new rectangle.</param>
        public RectF(Vector2 topLeft, SizeF rectSize)
            : this(topLeft.X, topLeft.Y, rectSize.Width, rectSize.Height)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the RectF structure that is defined by the given top
        ///  left and bottom right points.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="bottomRight">Bottom right corner of the new rectangle.</param>
        public RectF(Vector2 topLeft, Vector2 bottomRight)
            : this(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the RectF structure that is a copy of the provided
        ///  RectF.
        /// </summary>
        /// <param name="rect">The RectF instance to copy from.</param>
        public RectF(RectF rect)
            : this(rect.mX, rect.mY, rect.mWidth, rect.mHeight)
        {
        }

        /// <summary>
        ///  Get an empty rectangle.
        /// </summary>
        /// <remarks>
        ///  An empty rectangle is one anchored at (0,0) and has both the width and height set
        ///  to zero.
        /// </remarks>
        public static RectF Empty
        {
            [System.Diagnostics.DebuggerStepThrough] get { return EmptyRect; }
        }

        /// <summary>
        ///  Get the area of the rectangle.
        /// </summary>
        public float Area
        {
            [System.Diagnostics.DebuggerStepThrough] get { return Width * Height; }
        }

        /// <summary>
        ///  Get or set the left-most X position of the rectangle.
        /// </summary>
        [DataMember(Name = "x", Order = 0, IsRequired = true)]
        public float X
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mX; }
            [System.Diagnostics.DebuggerStepThrough] set { mX = value; }
        }

        /// <summary>
        ///  Get or set the top-most Y position of the rectangle.
        /// </summary>
        [DataMember(Name = "y", Order = 1, IsRequired = true)]
        public float Y
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mY; }
            [System.Diagnostics.DebuggerStepThrough] set { mY = value; }
        }

        /// <summary>
        ///  Get or set the width of the rectangle.
        /// </summary>
        [DataMember(Name = "width", Order = 2, IsRequired = true)]
        public float Width
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mWidth; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Width cannot be less than zero", "value");
                }

                mWidth = value;
            }
        }

        /// <summary>
        ///  Get or set the height of the rectangle.
        /// </summary>
        [DataMember(Name = "height", Order = 3, IsRequired = true)]
        public float Height
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mHeight; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Height cannot be less than zero", "value");
                }

                mHeight = value;
            }
        }

        /// <summary>
        ///  Get if the rectangle is empty. (eg, it has no area).
        /// </summary>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return mWidth == 0.0f || mHeight == 0.0f; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Get the maximum extent of the rectangle, which is the bottom right of the rectangle.
        /// </summary>
        public Vector2 MaxPoint
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY + mHeight); }
        }

        /// <summary>
        ///  Get the minimum extent of the rectangle, which is the top left of the rectangle.
        /// </summary>
        public Vector2 MinPoint
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX, mY); }
        }

        /// <summary>
        ///  Get the upper left position of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Position
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX, mY); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///  Get the upper left position of the rectangle.
        /// </summary>
        public Vector2 TopLeft
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX, mY); }
        }

        /// <summary>
        ///  Get the top center point of the rectangle.
        /// </summary>
        public Vector2 TopCenter
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth / 2.0f, mY); }
        }

        /// <summary>
        ///  Get the top right point of the rectangle.
        /// </summary>
        public Vector2 TopRight
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY); }
        }

        /// <summary>
        ///  Get the middle left point of the rectangle.
        /// </summary>
        public Vector2 MidLeft
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX, mY + mHeight / 2.0f); }
        }

        /// <summary>
        ///  Get the middle point of the rectangle.
        /// </summary>
        public Vector2 MidCenter
        {
            [System.Diagnostics.DebuggerStepThrough] get { return Center; }
        }

        /// <summary>
        ///  Get the middle right point of the rectangle.
        /// </summary>
        public Vector2 MidRight
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY + mHeight / 2.0f); }
        }

        /// <summary>
        ///  Get the bottom left point of the rectangle.
        /// </summary>
        public Vector2 BottomLeft
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX, mY + mHeight); }
        }

        /// <summary>
        ///  Get the bottom center point of the rectangle.
        /// </summary>
        public Vector2 BottomCenter
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth / 2.0f, mY + mHeight); }
        }

        /// <summary>
        ///  Get or set the bottom right of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 BottomRight
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY + mHeight); }
            set
            {
                Width = value.X - mX;
                Height = value.Y - mY;
            }
        }

        /// <summary>
        ///  Get or set the center of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Center
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new Vector2(mX + mWidth / 2.0f, mY + mHeight / 2.0f); }
            set
            {
                Vector2 halfSize = new Vector2( Width / 2.0f, Height / 2.0f );
                Position = value - halfSize;
            }
        }

        /// <summary>
        ///  Get or set the size of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public SizeF Size
        {
            [System.Diagnostics.DebuggerStepThrough] get { return new SizeF(mWidth, mHeight); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        ///  Get or set the top y coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Top
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mY; }
            [System.Diagnostics.DebuggerStepThrough] set { mY = value; }
        }

        /// <summary>
        ///  Get or set the left x coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Left
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mX; }
            [System.Diagnostics.DebuggerStepThrough] set { mX = value; }
        }

        /// <summary>
        ///  Get or set the right x coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Right
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mX + mWidth; }
            [System.Diagnostics.DebuggerStepThrough] set { Width = value - mX; }
        }

        /// <summary>
        ///  Get or set the bottom y coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Bottom
        {
            [System.Diagnostics.DebuggerStepThrough] get { return mY + mHeight; }
            [System.Diagnostics.DebuggerStepThrough] set { Height = value - mY; }
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
            return (X <= pointX) && (Y <= pointY) && (Right >= pointX) && (Bottom >= pointY);
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
        ///  Check if the given rectangle is contained inside of the rectangle.
        /// </summary>
        /// <param name="other">The rectangle to check against.</param>
        /// <returns>True if the other rectangle is contained inside of this rectangle.</returns>
        public bool Contains(RectF other)
        {
            return 
                (X <= other.X) &&
                (Y <= other.Y) &&
                (Right >= other.Right) &&
                (Bottom >= other.Bottom);
        }

        /// <summary>
        ///  Pushes the edges of the Rectangle out by the horizontal and vertical values specified.
        /// </summary>
        /// <remarks>
        ///  TODO: Make this more intelligent, as in give the ability to dictate how the rectangle will
        ///        grow.
        /// </remarks>
        /// <param name="horizontalAmount">Amount to inflate each side horizontally</param>
        /// <param name="verticalAmount">Amount to inflate each side vertically</param>
        public RectF Inflate(float horizontalAmount, float verticalAmount)
        {
            return new RectF(
                X - horizontalAmount,
                Y - verticalAmount,
                Width + horizontalAmount * 2.0f,
                Height + verticalAmount * 2.0f);
        }

        /// <summary>
        ///  Check if the given rectangle intersects this rectangle.
        /// </summary>
        /// <param name="other">The rectangle to check against.</param>
        /// <returns>True if the other rectangle intersects this rectangle.</returns>
        public bool Intersects(RectF other)
        {
            return 
                (X < other.Right) &&
                (Y < other.Bottom) &&
                (Right > other.X) &&
                (Bottom > other.Y);
        }

        /// <summary>
        ///  Check if the given rectangle intersects this rectangle and sets the ref result to
        ///  be the area that was intersected.
        /// </summary>
        /// <param name="other">The rectangle to check against.</param>
        /// <param name="result">Rectangle that will be set to the intersection area.</param>
        /// <returns>True if the other rectangle intersects this rectangle.</returns>
        public bool Intersects(RectF other, ref RectF result)
        {
            bool intersects = 
                (X < other.Right) && (Y < other.Bottom) &&
                (Right > other.X) && (Bottom > other.Y);

            if (intersects)
            {
                float aX = Math.Max(X, other.X);
                float aY = Math.Max(Y, other.Y);
                float bX = Math.Min(Right, other.Right);
                float bY = Math.Min(Bottom, other.Bottom);

                result.X = aX;
                result.Y = aY;
                result.Width = bX - aX;
                result.Height = bY - aY;
            }

            return intersects;
        }

        /// <summary>
        ///  Merge two rectangles into a single rectangle that is large enough to encompass both.
        /// </summary>
        /// <param name="first">The first rectangle to encompass.</param>
        /// <param name="second">The second rectangle to encompass.</param>
        /// <returns>The rectangle that encompassed both rectangles.</returns>       
        public static RectF Merge(RectF first, RectF second)
        {
            Vector2 min = Vector2.Min( first.TopLeft, second.TopLeft );
            Vector2 max = Vector2.Max( first.BottomRight, second.BottomRight );

            return new RectF(min, max);
        }

        /// <summary>
        ///  Move the rectangle by the given amount.
        /// </summary>
        /// <param name="amount">Amount to move the rectangle by.</param>
        public void Offset(Vector2 amount)
        {
            Offset(amount.X, amount.Y);
        }

        /// <summary>
        ///  Move the rectangle by the given amount.
        /// </summary>
        /// <param name="deltaX">X distance to move.</param>
        /// <param name="deltaY">Y distance to move.</param>
        public void Offset(float deltaX, float deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }

        /// <summary>
        ///  Check if another rectangle equals this rectangle's value.
        /// </summary>
        /// <returns>True if the rectangles have the same value, false otherwise.</returns>
        public bool Equals(RectF other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (X == other.X && Y == other.Y && Width == other.Width && Height == other.Height);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Check if another object equals this rectangle's value.
        /// </summary>
        /// <returns>True if the rectangles have the same value, false otherwise.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is RectF)
            {
                return Equals((RectF) obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Creates a rectangle that tightly contains the list of points.
        /// </summary>
        /// <param name="points">A list of points to contain.</param>
        /// <returns>A rectangle that contains all the given points.</returns>
        public static RectF Encapsulate(IList<Vector2> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            // UNTESTED
            // Don't bother encapsulating a rectangle when there are no points to
            // encapsulate.
            if (points.Count < 1)
            {
                throw new ArgumentException("There must be at least one point in the collection");
            }

            // Get the first elements values to use as defaults.
            Vector2 min = points[0];
            Vector2 max = points[1];

            for (int i = 1; i < points.Count; ++i)
            {
                min = Vector2.Min(points[i], min);
                max = Vector2.Max(points[i], max);
            }

            return new RectF(min, max - min);
        }

        /// <summary>
        ///  Get the object's string representation.
        /// </summary>
        /// <returns>Object's string representation.</returns>
        public override string ToString()
        {
            return String.Format(
                "x: {0}, y: {1}, w: {2}, h: {3}",
                X,
                Y,
                Width,
                Height);
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
                hash *= 23 + mWidth.GetHashCode();
                hash *= 23 + mHeight.GetHashCode();

                return hash;
            }
        }

#if USING_XNA
        public Microsoft.Xna.Rectangle ToRectangle()
        {
            return new Rectangle(
                (int) Math.Round( X ),
                (int) Math.Round( Y ),
                (int) Math.Round( Width ),
                (int) Math.Round( Height )
                );
        }
#endif

        public static bool operator ==(RectF left, RectF right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RectF left, RectF right)
        {
            return !(left == right);
        }

        public static RectF FromMinMaxPoint(float minX, float minY, float maxX, float maxY)
        {
            return new RectF(minX, minY, maxX - minX, maxY - minY);
        }

        public static RectF FromMinMaxPoint(Vector2 min, Vector2 max)
        {
            Vector2 dims = max - min;
            return new RectF(min, new SizeF(dims.X, dims.Y));
        }

    }
}
