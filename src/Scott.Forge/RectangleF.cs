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
using System.Runtime.Serialization;

namespace Scott.Forge
{
    /// <summary>
    /// Represents a rectangle using a floating point values
    /// </summary>
    [DataContract]
    [DebuggerDisplay( "X={X}, Y={Y}, Width={Width}, Height={Height}" )]
    public struct RectangleF : IEquatable<RectangleF>
    {
        /// <summary>
        ///  Left-most X coordinate of the rectangle.
        /// </summary>
        [DataMember]
        public float X { get; set; }

        /// <summary>
        ///  Top-most Y coordinate of the rectangle.
        /// </summary>
        [DataMember]
        public float Y { get; set; }

        /// <summary>
        ///  Width of the rectangle.
        /// </summary>
        [DataMember]
        public float Width { get; set; }

        /// <summary>
        ///  Height of the rectangle.
        /// </summary>
        [DataMember]
        public float Height { get; set; }

        /// <summary>
        ///  Left-most X coordinate of the rectangle.
        /// </summary>
        public float Left
        {
            [DebuggerStepThrough]
            get
            {
                return X;
            }
        }

        /// <summary>
        ///  Right most X coordinate of the rectangle.
        /// </summary>
        public float Right
        {
            [DebuggerStepThrough]
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        ///  Top most Y coordinate of the rectangle.
        /// </summary>
        public float Top
        {
            [DebuggerStepThrough]
            get
            {
                return Y;
            }
        }

        /// <summary>
        ///  Bottom most Y coordinate of the rectangle.
        /// </summary>
        public float Bottom
        {
            [DebuggerStepThrough]
            get
            {
                return Y + Height;
            }
        }

        /// <summary>
        ///  Upper left (X,Y) position of the rectangle
        /// </summary>
        public Vector2 Location
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X, Y );
            }
        }

        /// <summary>
        ///  Width and height of the rectangle
        /// </summary>
        public Vector2 Size
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( Width, Height );
            }
        }

        /// <summary>
        ///  Center (X,Y) position of the rectangle
        /// </summary>
        public Vector2 Center
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X + Width / 2.0f, Y + Height / 2.0f );
            }
        }

        /// <summary>
        ///  Top left point of the rectangle
        /// </summary>
        public Vector2 TopLeft
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X, Y );
            }
        }

        /// <summary>
        ///  Top right point of the rectangle
        /// </summary>
        public Vector2 TopRight
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X + Width, Y );
            }
        }

        /// <summary>
        ///  Bottom left point of the rectangle
        /// </summary>
        public Vector2 BottomLeft
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X, Y + Height );
            }
        }

        /// <summary>
        ///  Bottom right point of the rectangle
        /// </summary>
        public Vector2 BottomRight
        {
            [DebuggerStepThrough]
            get
            {
                return new Vector2( X + Width, Y + Height );
            }
        }

        /// <summary>
        ///  Tests if the rectangle has zero width or height.
        /// </summary>
        public bool IsEmpty
        {
            [DebuggerStepThrough]
            get
            {
                return ( Width == 0.0f || Height == 0.0f );
            }
        }

        /// <summary>
        ///  Returns an empty rectangle with a zero width and height.
        /// </summary>
        public static readonly RectangleF Empty = new RectangleF( 0.0f, 0.0f, 0.0f, 0.0f );

        /// <summary>
        ///  Rectangle constructor.
        /// </summary>
        /// <param name="x">Left most x position of the rectangle</param>
        /// <param name="y">Top most y position of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public RectangleF( float x, float y, float width, float height )
            : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///  Rectangle constructor.
        /// </summary>
        /// <param name="topLeft">Top left position of the rectangle</param>
        /// <param name="size">Width and height of the rectangle</param>
        public RectangleF( Vector2 topLeft, Vector2 size )
            : this()
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = size.X;
            Height = size.Y;
        }
    /*
        /// <summary>
        ///  Rectangle constructor.
        /// </summary>
        /// <param name="r">XNA rectangle</param>
        public RectangleF( Microsoft.Xna.Framework.Rectangle r )
            : this()
        {
            X = (float) r.X;
            Y = (float) r.Y;
            Width = (float) r.Width;
            Height = (float) r.Height;
        }*/

        /// <summary>
        ///  Copy constructor
        /// </summary>
        /// <param name="r">RectangeF to copy</param>
        public RectangleF( RectangleF r )
            : this( r.X, r.Y, r.Width, r.Height )
        {
        }
         
        /*
        /// <summary>
        ///  Returns an XNA representation of this RectangleF.
        /// </summary>
        /// <remarks>
        ///  The floating point values used to represent the rectangle's coordinates are rounded to
        ///  the nearest integer when creating the XNA rectangle. The instance itself is not
        ///  modified.
        /// </remarks>
        /// <returns>XNA rectangle</returns>
        public Microsoft.Xna.Framework.Rectangle ToRectangle()
        {
            return new Rectangle(
                (int) Math.Round( X ),
                (int) Math.Round( Y ),
                (int) Math.Round( Width ),
                (int) Math.Round( Height )
            );
        }
         */

        /// <summary>
        ///  Test if the provided point is inside the rectangle.
        /// </summary>
        /// <param name="x">X coordinate to test.</param>
        /// <param name="y">Y coordinate to test.</param>
        /// <returns>True if the point is contained in the rectangle, false otherwise.</returns>
        public bool Contains( float x, float y )
        {
            return ( X <= x ) && ( Y <= y ) && ( Right >= x ) && ( Bottom >= y );
        }

        /// <summary>
        ///  Test if the provided point is inside the rectangle.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns>True if the point is contained in the rectangle, false otherwise.</returns>
        public bool Contains( Vector2 pt )
        {
            return Contains( pt.X, pt.Y );
        }

        /// <summary>
        ///  Test if the provided rectangle is inside this rectangle.
        /// </summary>
        /// <param name="rhs">The rectangle to test against.</param>
        /// <returns>True if this rectangle fully contains the provided rectangle.</returns>
        public bool Contains( RectangleF rhs )
        {
            return ( X <= rhs.X ) &&
                   ( Y <= rhs.Y ) &&
                   ( Right >= rhs.Right ) &&
                   ( Bottom >= rhs.Bottom );
        }

        /// <summary>
        ///  Pushes the edges of the Rectangle out by the horizontal and vertical values specified.
        /// </summary>
        /// <param name="horizontalAmount">Amount to inflate each side horizontally</param>
        /// <param name="verticalAmount">Amount to inflate each side vertically</param>
        public void Inflate( float horizontalAmount, float verticalAmount )
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2.0f;
            Height += verticalAmount * 2.0f;
        }

        /// <summary>
        ///  Tests if the provided rectangle intersects this rectangle.
        /// </summary>
        /// <param name="rhs">The rectangle to test against.</param>
        /// <returns>True if the provided rectangle intersects this rectangle.</returns>
        public bool Intersects( RectangleF rhs )
        {
            return ( X < rhs.Right ) &&
                   ( Y < rhs.Bottom ) &&
                   ( Right > rhs.X ) &&
                   ( Bottom > rhs.Y );
        }

        /// <summary>
        ///  Tests if the provided rectangle intersects this rectangle.
        /// </summary>
        /// <param name="rhs">The rectangle to test against.</param>
        /// <param name="result">
        ///  If the rectangle intersects, this parameter will a rectangle that encompasses the
        ///  intersecting area.
        /// </param>
        /// <returns>True if the provided rectangle intersects this rectangle.</returns>
        public bool Intersects( RectangleF rhs, ref RectangleF result )
        {
            bool intersects = ( X < rhs.Right ) && ( Y < rhs.Bottom ) &&
                              ( Right > rhs.X ) && ( Bottom > rhs.Y );

            if ( intersects )
            {
                float aX = Math.Max( X, rhs.X );
                float aY = Math.Max( Y, rhs.Y );
                float bX = Math.Min( Right, rhs.Right );
                float bY = Math.Min( Bottom, rhs.Bottom );

                result.X = aX;
                result.Y = aY;
                result.Width = bX - aX;
                result.Height = bY - aY;
            }

            return intersects;
        }

        /// <summary>
        ///  Moves the rectangle's position relative to where it is currently located.
        /// </summary>
        /// <param name="amount">Distance to move the rectangle.</param>
        public void Offset( Vector2 amount )
        {
            Offset( amount.X, amount.Y );
        }

        /// <summary>
        ///  Moves the rectangle's position relative to where it is currently located.
        /// </summary>
        /// <param name="deltaX">Amount of horizontal distance to move the rectangle.</param>
        /// <param name="deltaY">Amount of vertical distance to move the rectangle.</param>
        public void Offset( float deltaX, float deltaY )
        {
            X += deltaX;
            Y += deltaY;
        }

        /// <summary>
        ///  RectangleF equality operator. Compares this rectangle to the provided rectangle to see
        ///  if they have identical values.
        /// </summary>
        /// <param name="rhs">Rectangle to compare against.</param>
        /// <returns>True if the rectangles have identical values, false otherwise.</returns>
        public bool Equals( RectangleF rhs )
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            return ( X == rhs.X && Y == rhs.Y && Width == rhs.Width && Height == rhs.Height );
// ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  RectangleF equality operator.
        /// </summary>
        /// <param name="lhs">Rectangle to compare.</param>
        /// <param name="rhs">Other rectangle to compare.</param>
        /// <returns>True if the rectangles have identical values, false otherwise.</returns>
        public static bool operator ==( RectangleF lhs, RectangleF rhs )
        {
            return lhs.Equals( rhs );
        }

        /// <summary>
        ///  RectangleF inequality operator.
        /// </summary>
        /// <param name="lhs">Rectangle to compare.</param>
        /// <param name="rhs">Other rectangle to compare.</param>
        /// <returns>True if the rectangles do not have identical values, false otherwise.</returns>
        public static bool operator !=( RectangleF lhs, RectangleF rhs )
        {
            return !lhs.Equals( rhs );
        }

        /// <summary>
        ///  Overridden ToString method. Returns a JSON compatible description of the rectangle
        ///  dimensions.
        /// </summary>
        /// <returns>A string with the rectangle's values.</returns>
        public override string ToString()
        {
            return
                String.Format( "{{ x = {0}, y = {1}, w = {2}, h = {3} }}", X, Y, Width, Height );
        }

        /// <summary>
        ///  Overriden RectangleF equality operator
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <returns></returns>
        public override bool Equals( Object obj )
        {
            if ( obj is RectangleF )
            {
                return Equals( (RectangleF) obj );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Calculate hash code for this rectangle.
        /// </summary>
        /// <returns>Rectangle's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                hash *= 23 + X.GetHashCode();
                hash *= 23 + Y.GetHashCode();
                hash *= 23 + Width.GetHashCode();
                hash *= 23 + Height.GetHashCode();

                return hash;
            }
        }
    }
}
