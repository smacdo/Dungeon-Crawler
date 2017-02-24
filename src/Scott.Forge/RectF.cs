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
using System.Diagnostics;
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
        ///  Initializes a new instance of the RectF structure that is defined by the provided x, y, width and height
        ///  values.
        /// </summary>
        /// <param name="top">Top X coordinate for the rectangle.</param>
        /// <param name="left">Top Y coordinate for the rectangle.</param>
        /// <param name="width">Width of the new rectangle.</param>
        /// <param name="height">Height of the new rectangle.</param>
        public RectF(float top, float left, float width, float height)
        {
            if (width < 0.0f)
            {
                throw new ArgumentException("Width cannot be less than zero", nameof(width));
            }

            if (height < 0.0f)
            {
                throw new ArgumentException("Height cannot be less than zero", nameof(height));
            }

            mX = top;
            mY = left;
            mWidth = width;
            mHeight = height;
        }

        /// <summary>
        ///  Initializes a new instance of the Rect structure that has the specified top left corner and the
        ///  given width and height.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="rectSize">Size of the new rectangle.</param>
        public RectF(Vector2 topLeft, SizeF rectSize)
            : this(
                  top: topLeft.X,
                  left: topLeft.Y,
                  width: rectSize.Width,
                  height: rectSize.Height)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the RectF structure that is defined by the given top left and bottom right
        ///  points.
        /// </summary>
        /// <param name="topLeft">Top left corner of the new rectangle.</param>
        /// <param name="bottomRight">Bottom right corner of the new rectangle.</param>
        public RectF(Vector2 topLeft, Vector2 bottomRight)
            : this(
                  top: topLeft.X,
                  left: topLeft.Y,
                  width: bottomRight.X - topLeft.X,
                  height: bottomRight.Y - topLeft.Y)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the RectF structure that is a copy of the provided RectF object.
        /// </summary>
        /// <param name="rect">The RectF object to copy from.</param>
        public RectF(RectF rect)
            : this(
                  top: rect.mX,
                  left: rect.mY,
                  width: rect.mWidth,
                  height: rect.mHeight)
        {
        }

        /// <summary>
        ///  Get an empty rectangle.
        /// </summary>
        /// <remarks>
        ///  An empty rectangle has a top left position of (0, 0) and both the width and height are zero.
        /// </remarks>
        public static RectF Empty
        {
            [DebuggerStepThrough] get { return EmptyRect; }
        }

        /// <summary>
        ///  Get the area of the rectangle.
        /// </summary>
        public float Area
        {
            [DebuggerStepThrough] get { return Width * Height; }
        }

        /// <summary>
        ///  Get or set the left-most X position of the rectangle.
        /// </summary>
        [DataMember(Name = "x", Order = 0, IsRequired = true)]
        public float X
        {
            [DebuggerStepThrough] get { return mX; }
            [DebuggerStepThrough] set { mX = value; }
        }

        /// <summary>
        ///  Get or set the top-most Y position of the rectangle.
        /// </summary>
        [DataMember(Name = "y", Order = 1, IsRequired = true)]
        public float Y
        {
            [DebuggerStepThrough] get { return mY; }
            [DebuggerStepThrough] set { mY = value; }
        }

        /// <summary>
        ///  Get or set the width of the rectangle.
        /// </summary>
        [DataMember(Name = "width", Order = 2, IsRequired = true)]
        public float Width
        {
            [DebuggerStepThrough] get { return mWidth; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Width cannot be less than zero", nameof(Width));
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
            [DebuggerStepThrough] get { return mHeight; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentException("Height cannot be less than zero", nameof(Height));
                }

                mHeight = value;
            }
        }

        /// <summary>
        ///  Get if the rectangle is empty.
        /// </summary>
        /// <remarks>
        ///  A rectangle is considered empty when either its width or height is equal to zero.
        /// </remarks>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return mWidth == 0.0f || mHeight == 0.0f; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Get or set the upper left position of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 TopLeft
        {
            [DebuggerStepThrough] get { return new Vector2(mX, mY); }
            [DebuggerStepThrough]
            set
            {
                mX = value.X;
                mY = value.Y;
            }
        }

        /// <summary>
        ///  Get the top center point of the rectangle.
        /// </summary>
        public Vector2 TopCenter
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth * 0.5f, mY); }
        }

        /// <summary>
        ///  Get the top right point of the rectangle.
        /// </summary>
        public Vector2 TopRight
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY); }
        }

        /// <summary>
        ///  Get the middle left point of the rectangle.
        /// </summary>
        public Vector2 MidLeft
        {
            [DebuggerStepThrough] get { return new Vector2(mX, mY + mHeight / 2.0f); }
        }

        /// <summary>
        ///  Get the middle point of the rectangle.
        /// </summary>
        public Vector2 MidCenter
        {
            [DebuggerStepThrough] get { return Center; }
        }

        /// <summary>
        ///  Get the middle right point of the rectangle.
        /// </summary>
        public Vector2 MidRight
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY + mHeight / 2.0f); }
        }

        /// <summary>
        ///  Get the bottom left point of the rectangle.
        /// </summary>
        public Vector2 BottomLeft
        {
            [DebuggerStepThrough] get { return new Vector2(mX, mY + mHeight); }
        }

        /// <summary>
        ///  Get the bottom center point of the rectangle.
        /// </summary>
        public Vector2 BottomCenter
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth / 2.0f, mY + mHeight); }
        }

        /// <summary>
        ///  Get or set the bottom right of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 BottomRight
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth, mY + mHeight); }
            set
            {
                // TODO: This operation is not well defined. Does the rectangle expand in size to cover the new
                //       bottom right? Or does the top left change to preserve the size but move such that this is
                //       the new bottom right? Even if explained in the remarks neither operation is more compelling
                //       than the other.
                Width = value.X - mX;
                Height = value.Y - mY;
            }
        }

        /// <summary>
        ///  Get or set the center of the rectangle.
        /// </summary>
        /// <remarks>
        ///  Changing the center of the rectangle will move the top left position rather than change the size of the
        ///  rectangle.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public Vector2 Center
        {
            [DebuggerStepThrough] get { return new Vector2(mX + mWidth / 2.0f, mY + mHeight / 2.0f); }
            [DebuggerStepThrough]
            set
            {
                var halfSize = new Vector2( Width / 2.0f, Height / 2.0f );
                TopLeft = value - halfSize;
            }
        }

        /// <summary>
        ///  Get or set the size of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public SizeF Size
        {
            [DebuggerStepThrough] get { return new SizeF(mWidth, mHeight); }
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
            [DebuggerStepThrough] get { return mY; }
            [DebuggerStepThrough] set { mY = value; }
        }

        /// <summary>
        ///  Get or set the left x coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Left
        {
            [DebuggerStepThrough] get { return mX; }
            [DebuggerStepThrough] set { mX = value; }
        }

        /// <summary>
        ///  Get or set the right x coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Right
        {
            [DebuggerStepThrough] get { return mX + mWidth; }
            [DebuggerStepThrough] set { Width = value - mX; }
        }

        /// <summary>
        ///  Get or set the bottom y coordinate of the rectangle.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public float Bottom
        {
            [DebuggerStepThrough] get { return mY + mHeight; }
            [DebuggerStepThrough] set { Height = value - mY; }
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
        /// <summary>
        ///  Return a XNA Rectangle instance with the same values as this object.
        /// </summary>
        /// <returns>XNA Rectangle with same values as this object.</returns>
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

        /// <summary>
        ///  Equality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>True if the rectangles are equal in value, false otherwise.</returns>
        public static bool operator ==(RectF left, RectF right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///  Inequality operator.
        /// </summary>
        /// <param name="left">Left hand side.</param>
        /// <param name="right">Right hand side.</param>
        /// <returns>True if the rectangles are not equal in value, false otherwise.</returns>
        public static bool operator !=(RectF left, RectF right)
        {
            return !(left == right);
        }

        /// <summary>
        ///  Calculate the minimum displacement vector for two axis aligned rectangles.
        /// </summary>
        /// <remarks>
        ///  The displacement vector will be aligned on either the X or Y axis and is used to displace the first
        ///  rectangle enough to no longer instersect the second rectangle.
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 FindMinimumDisplacement(RectF a, RectF b)
        {
            var result = Vector2.Zero;
            
            var left = b.Left - a.Right;
            var right = b.Right - a.Left;
            var bottom = b.Top - a.Bottom;
            var top = b.Bottom - a.Top;

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
