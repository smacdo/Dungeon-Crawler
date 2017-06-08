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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Scott.Forge
{
    /// <summary>
    ///  A two dimensional Point with integer coordinates.
    /// </summary>
    /// <remarks>
    ///  Do not use this mutable struct when code (Especially collections like IDictionary) expects the hash code to
    ///  remain immutable but the properties of the struct are modified.
    /// </remarks>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("X={X}, Y={Y}")]
    public struct Point2 : IEquatable<Point2>, IComparable<Point2>, IComparable
    {
        private static readonly Point2 PointDown  = new Point2(0, 1);
        private static readonly Point2 PointLeft  = new Point2(-1, 0);
        private static readonly Point2 PointOne   = new Point2(1, 1);
        private static readonly Point2 PointRight = new Point2(1, 0);
        private static readonly Point2 PointUp    = new Point2(0, -1);
        private static readonly Point2 PointZero  = new Point2(0, 0);
        
        /// <summary>
        ///  Initializes a new instance of the Point structure.
        /// </summary>
        /// <param name="x">The x offset of the new Point.</param>
        /// <param name="y">The y offset of the new Point.</param>
        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///  Initializes a new instance of the Point structure.
        /// </summary>
        /// <param name="other">A Point to copy values from.</param>
        public Point2(Point2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        /// <summary>
        ///  Get a unit Point that points down.
        /// </summary>
        public static Point2 Down
        {
            get { return PointDown; }
        }

        /// <summary>
        ///  Get a unit Point that points left.
        /// </summary>
        public static Point2 Left
        {
            get { return PointLeft; }
        }

        /// <summary>
        ///  Get a Point that has X and Y set to one.
        /// </summary>
        public static Point2 One
        {
            get { return PointOne; }
        }

        /// <summary>
        ///  Get a unit Point that points right.
        /// </summary>
        public static Point2 Right
        {
            get { return PointRight; }
        }

        /// <summary>
        ///  Get a unit Point that points up.
        /// </summary>
        public static Point2 Up
        {
            get { return PointUp; }
        }

        /// <summary>
        ///  Get a Point that has X and Y set to zero.
        /// </summary>
        public static Point2 Zero
        {
            get { return PointZero; }
        }

        /// <summary>
        ///  Get or set the X component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "x", Order = 0, IsRequired = true)]
        public int X { get; set; }

        /// <summary>
        ///  Get or set the Y component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "y", Order = 1, IsRequired = true)]
        public int Y { get; set; }

        /// <summary>
        ///  Get if the X and Y of this Point are both zero.
        /// </summary>
        public bool IsZero
        {
            get { return X == 0 && Y == 0; }
        }

        /// <summary>
        ///  Compare this Point to another Point.
        /// </summary>
        /// <returns>-1 if this point is smaller, 1 if larger and 0 if they are equal. </returns>
        public int CompareTo(Point2 other)
        {
            // Perform comparison in such a way to order a set of points in row major order. This means that y should
            // be sorted first so that points with the same Y value are grouped together.
            if (Y < other.Y)
            {
                return -1;
            }
            else if (Y > other.Y)
            {
                return 1;
            }
            else
            {
                if (X < other.X)
                {
                    return -1;
                }
                else if (X > other.X)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        ///  Compare this Point to another Point.
        /// </summary>
        /// <returns>-1 if this point is smaller, 1 if larger and 0 if they are equal. </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj is Point2)
            {
                return CompareTo((Point2) obj);
            }
            else
            {
                throw new ArgumentException("Cannot compare different types", nameof(obj)); 
            }
        }

        /// <summary>
        ///  Check if another Point is equal to this Point.
        /// </summary>
        /// <returns>True if the Point is equal, false otherwise.</returns>
        public bool Equals(Point2 other)
        {
            return (X == other.X && Y == other.Y);
        }

        /// <summary>
        ///  Check if another object is equal to this Point.
        /// </summary>
        /// <returns>True if the other object is a Point and equal to this Point.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Point2)
            {
                return Equals((Point2) obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Get this object's numeric value that can be used to insert and identify an object in a hash based
        ///  collection.
        /// </summary>
        /// <returns>This object's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 23) + X.GetHashCode();
                hash = (hash * 23) + Y.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///  Return a negated version of this Point.
        /// </summary>
        /// <returns>A Point with the X and Y components negated.</returns>
        public Point2 Negated()
        {
            return new Point2(-X, -Y);
        }

        /// <summary>
        ///  Returns a string that represents this Point.
        /// </summary>
        /// <returns>A string that represents this Point.</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }

        /// <summary>
        ///  Unary positive operator.
        /// </summary>
        public static Point2 operator +(Point2 Point)
        {
            return new Point2(Point.X, Point.Y);
        }

        /// <summary>
        ///  Unary negative operator.
        /// </summary>
        public static Point2 operator -(Point2 Point)
        {
            return new Point2(-Point.X, -Point.Y);
        }

        /// <summary>
        ///  Addition operator.
        /// </summary>
        public static Point2 Add(Point2 left, Point2 right)
        {
            return new Point2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///  Addition operator.
        /// </summary>
        public static Point2 operator +(Point2 left, Point2 right)
        {
            return new Point2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///  Subtraction operator.
        /// </summary>
        public static Point2 Subtract(Point2 left, Point2 right)
        {
            return new Point2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///  Subtraction operator.
        /// </summary>
        public static Point2 operator -(Point2 left, Point2 right)
        {
            return new Point2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///  Scalar multiplication operator.
        /// </summary>
        public static Point2 operator *(Point2 left, int right)
        {
            return new Point2(left.X * right, left.Y * right);
        }

        /// <summary>
        ///  Scalar multiplication operator.
        /// </summary>
        public static Point2 Multiply(Point2 left, int right)
        {
            return new Point2(left.X * right, left.Y * right);
        }

        /// <summary>
        ///  Scalar division operator.
        /// </summary>
        public static Point2 operator /(Point2 left, int right)
        {
            return new Point2(left.X / right, left.Y / right);
        }

        /// <summary>
        ///  Scalar division operator.
        /// </summary>
        public static Point2 Divide(Point2 left, int right)
        {
            return new Point2(left.X / right, left.Y / right);
        }

        /// <summary>
        ///  Exact equality operator.
        /// </summary>
        public static bool operator ==(Point2 left, Point2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///  Exact inequality operator.
        /// </summary>
        public static bool operator !=(Point2 left, Point2 right)
        {
            return !(left.Equals(right));
        }
        
        /// <summary>
        ///  Calculates the distance of two Points.
        /// </summary>
        /// <param name="left">First Point.</param>
        /// <param name="right">Second Point.</param>
        /// <returns>Distance of the two Points.</returns>
        public static float Distance(Point2 left, Point2 right)
        {
            return (float) System.Math.Sqrt(
                (left.X - right.X) * (left.X - right.X) +
                (left.Y - right.Y) * (left.Y - right.Y));
        }

        /// <summary>
        ///  Calculates the distance squared of two Points.
        /// </summary>
        /// <param name="left">First Point.</param>
        /// <param name="right">Second Point.</param>
        /// <returns>Distance squared of the two Points.</returns>
        public static float DistanceSquared(Point2 left, Point2 right)
        {
            return ((left.X - right.X) * (left.X - right.X) + (left.Y - right.Y) * (left.Y - right.Y));
        }
        
        /// <summary>
        ///  Calculates a Point with maximum X and Y component of two Points.
        /// </summary>
        /// <param name="left">First Point.</param>
        /// <param name="right">Second Point.</param>
        /// <returns>A Point containing the maximum X and Y component.</returns>
        public static Point2 Max(Point2 left, Point2 right)
        {
            return new Point2(System.Math.Max(left.X, right.X), System.Math.Max(left.Y, right.Y));
        }

        /// <summary>
        ///  Calculates a Point with minimum X and Y component of two Points.
        /// </summary>
        /// <param name="left">First Point.</param>
        /// <param name="right">Second Point.</param>
        /// <returns>A Point containing the minimum X and Y component.</returns>
        public static Point2 Min(Point2 left, Point2 right)
        {
            return new Point2(System.Math.Min(left.X, right.X), System.Math.Min(left.Y, right.Y));
        }

        /// <summary>
        ///  Negates a Point by mutliplying the X and Y by -1.0f.
        /// </summary>
        /// <param name="Point">The Point to negate.</param>
        /// <returns>The negated input Point.</returns>
        public static Point2 Negate(Point2 Point)
        {
            return new Point2(-Point.X, -Point.Y);
        }
    }
}