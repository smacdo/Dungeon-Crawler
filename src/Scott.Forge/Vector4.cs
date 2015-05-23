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
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Scott.Forge
{
    /// <summary>
    ///  A three dimensional X/Y/Z vector that captures offset and distance from origin.
    /// </summary>
    [DataContract]
    [System.Diagnostics.DebuggerDisplay("X={X}, Y={Y}, Z={Z}, W={W}")]
    public struct Vector4 : IEquatable<Vector4>, IFormattable
    {
        private static readonly Vector4 VectorZero  = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        private float mX;
        private float mY;
        private float mZ;
        private float mW;

        /// <summary>
        ///  Initializes a new instance of the Vector structure.
        /// </summary>
        /// <param name="x">The x offset of the new vector.</param>
        /// <param name="y">The y offset of the new vector.</param>
        public Vector4(float x, float y, float z, float w)
        {
            mX = x;
            mY = y;
            mZ = z;
            mW = w;
        }

        /// <summary>
        ///  Initializes a new instance of the Vector structure.
        /// </summary>
        /// <param name="other">A vector to copy values from.</param>
        public Vector4(Vector4 other)
        {
            mX = other.mX;
            mY = other.mY;
            mZ = other.mZ;
            mW = other.mW;
        }

        /// <summary>
        ///  Get a vector that has X and Y set to zero.
        /// </summary>
        public static Vector4 Zero
        {
            get { return VectorZero; }
        }

        /// <summary>
        ///  Get or set the X component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "x", Order = 0, IsRequired = true)]
        public float X
        {
            get { return mX; }
            set { mX = value; }
        }

        /// <summary>
        ///  Get or set the Y component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "y", Order = 1, IsRequired = true)]
        public float Y
        {
            get { return mY; }
            set { mY = value; }
        }

        /// <summary>
        ///  Get or set the Z component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "z", Order = 2, IsRequired = true)]
        public float Z
        {
            get { return mZ; }
            set { mZ = value; }
        }

        /// <summary>
        ///  Get or set the Z component.
        /// </summary>
        [XmlAttribute]
        [DataMember(Name = "w", Order = 3, IsRequired = true)]
        public float W
        {
            get { return mW; }
            set { mW = value; }
        }

        /// <summary>
        ///  Get the length (magnitude) of this vector.
        /// </summary>
        public float Length
        {
            get { return (float) Math.Sqrt(mX * mX + mY * mY + mZ * mZ + mW * mW); }
        }

        /// <summary>
        ///  Get the length squared of this vector.
        /// </summary>
        public float LengthSquared
        {
            get { return mX * mX + mY * mY + mZ * mZ + mW * mW; }
        }

        /// <summary>
        ///  Get if the X and Y of this vector are both zero.
        /// </summary>
        public bool IsZero
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return mX == 0.0f && mY == 0.0f && mZ == 0.0f && mW == 0.0f; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Return a negated version of this vector.
        /// </summary>
        /// <returns>A vector with the X and Y components negated.</returns>
        public Vector4 Negated()
        {
            return new Vector4(-X, -Y, -Z, -W);
        }

        /// <summary>
        ///  Normalize this vector.
        /// </summary>
        public void Normalize()
        {
            float length = Length;

            if (length == 0.0f)
            {
                throw new InvalidOperationException("Cannot normalize a Vector4 of length zero");
            }
            else
            {
                X = X / length;
                Y = Y / length;
                Z = Z / length;
                W = W / length;
            }
        }

        /// <summary>
        ///  Returns a copy of this vector that is normalized.
        /// </summary>
        /// <returns>Normalized copy of this vector.</returns>
        public Vector4 Normalized()
        {
            float length = Length;

            if (length == 0.0f)
            {
                throw new InvalidOperationException("Cannot normalize a Vector4 of length zero");
            }
            else
            {
                return new Vector4(X / length, Y / length, Z / length, W / length);
            }
        }

        /// <summary>
        ///  Returns a string that represents this vector.
        /// </summary>
        /// <returns>A string that represents this vector.</returns>
        public override string ToString()
        {
            return ToString("G", null);
        }

        /// <summary>
        ///  Returns a string that represents this vector.
        /// </summary>
        /// <param name="format">Format to use.</param>
        /// <returns>A string that represents this vector.</returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        ///  Returns a string that represents this vector.
        /// </summary>
        /// <remarks>
        ///  The default format 'G' returns the two components along with a X: and Y: prefix on
        ///  the values inside angle brackets. (eg, "&lt;x: 5, y: 3&gt;").
        ///
        ///  'F' returns a similiar string, however "Vector4" is included directly after the
        ///  opening bracket. (eg, "&lt;Vector4 x: 5, y: 3&gt;)".
        ///
        ///  'J' is JSON object notation. { "x": "5", "y": "3" }
        ///
        ///  'V' is the two values separated by a comma "5, 4".
        /// </remarks>
        /// <param name="format">The format representation to use.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>String representation of this vector.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // Use the default format if a format was not provided.
            if (String.IsNullOrEmpty(format))
            {
                format = "G";
            }

            // Use the default format provider if none were provided.
            if (formatProvider == null)
            {
                formatProvider = NumberFormatInfo.CurrentInfo;
            }

            // Format correctly based on requested format.
            switch (format)
            {
                case "G":
                    return String.Format("<x: {0}, y: {1}, z: {2}, w: {3}>", X, Y, Z, W);

                case "F":
                    return String.Format("<Vector4 x: {0}, y: {1}, z: {2}, w: {3}>", X, Y, Z, W);

                case "J":
                    return String.Format(
                        "{{ \"x\": \"{0}\", \"y\": \"{1}\", \"z\": \"{2}\", \"w\": \"{3}\" }}",
                        X,
                        Y,
                        Z,
                        W);

                case "V":
                    return String.Format("{0}, {1}, {2}, {3}", X, Y, Z, W);

                default:
                    throw new FormatException(
                        String.Format("Format string '{0}' is not supported", format));
            }
        }

        /// <summary>
        ///  Check if another vector is equal to this vector.
        /// </summary>
        /// <remarks>
        ///  This performs an exact quality test, so if the two vectors are ALMOST equal due to
        ///  floating point inaccuracy, the equality check will fail.
        /// </remarks>
        /// <returns>True if the vector is equal, false otherwise.</returns>
        public bool Equals(Vector4 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (mX == other.mX && mY == other.mY && mZ == other.mZ && mW == other.mW);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///  Check if another object is equal to this vector.
        /// </summary>
        /// <remarks>
        ///  This performs an exact quality test, so if the two vectors are ALMOST equal due to
        ///  floating point inaccuracy, the equality check will fail.
        /// </remarks>
        /// <returns>True if the other object is a vector and equal to this vector.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            {
                return Equals((Vector4) obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Get this object's numeric value that can be used to insert and identify an object in a
        ///  hash based collection.
        /// </summary>
        /// <returns>This object's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 269;

                hash = (hash * 47) + mX.GetHashCode();
                hash = (hash * 47) + mY.GetHashCode();
                hash = (hash * 47) + mZ.GetHashCode();
                hash = (hash * 47) + mW.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///  Unary positive operator.
        /// </summary>
        public static Vector4 operator +(Vector4 vector)
        {
            return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }

        /// <summary>
        ///  Unary negative operator.
        /// </summary>
        public static Vector4 operator -(Vector4 vector)
        {
            return new Vector4(-vector.X, -vector.Y, -vector.Z, -vector.W);
        }

        /// <summary>
        ///  Addition operator.
        /// </summary>
        public static Vector4 Add(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        ///  Addition operator.
        /// </summary>
        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        ///  Subtraction operator.
        /// </summary>
        public static Vector4 Subtract(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        ///  Subtraction operator.
        /// </summary>
        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        ///  Scalar multiplication operator.
        /// </summary>
        public static Vector4 operator *(Vector4 left, float right)
        {
            return new Vector4(left.X * right, left.Y * right, left.Z * right, left.W * right);
        }

        /// <summary>
        ///  Scalar multiplication operator.
        /// </summary>
        public static Vector4 Multiply(Vector4 left, float right)
        {
            return new Vector4(left.X * right, left.Y * right, left.Z * right, left.W * right);
        }

        /// <summary>
        ///  Scalar division operator.
        /// </summary>
        public static Vector4 operator /(Vector4 left, float right)
        {
            return new Vector4(left.X / right, left.Y / right, left.Z / right, left.W / right);
        }

        /// <summary>
        ///  Scalar division operator.
        /// </summary>
        public static Vector4 Divide(Vector4 left, float right)
        {
            return new Vector4(left.X / right, left.Y / right, left.Z / right, left.W / right);
        }

        /// <summary>
        ///  Exact equality operator.
        /// </summary>
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///  Exact inequality operator.
        /// </summary>
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left.Equals(right));
        }

        /// <summary>
        ///  Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>Cross product of the two vectors.</returns>
        public static Vector4 Cross(Vector4 left, Vector4 right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Calculates the distance of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>Distance of the two vectors.</returns>
        public static float Distance(Vector4 left, Vector4 right)
        {
            return (float) Math.Sqrt(
                (left.X - right.X) * (left.X - right.X) +
                (left.Y - right.Y) * (left.Y - right.Y) +
                (left.Z - right.Z) * (left.Z - right.Z) +
                (left.W - right.W) * (left.W - right.W));
        }

        /// <summary>
        ///  Calculates the distance squared of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>Distance squared of the two vectors.</returns>
        public static float DistanceSquared(Vector4 left, Vector4 right)
        {
            return (left.X - right.X) * (left.X - right.X) +
                   (left.Y - right.Y) * (left.Y - right.Y) +
                   (left.Z - right.Z) * (left.Z - right.Z) +
                   (left.W - right.W) * (left.W - right.W);
        }

        /// <summary>
        ///  Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>Dot product of the two vectors.</returns>
        public static float Dot(Vector4 left, Vector4 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

        /// <summary>
        ///  Calculates a vector with maximum X and Y component of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>A vector containing the maximum X and Y component.</returns>
        public static Vector4 Max(Vector4 left, Vector4 right)
        {
            return new Vector4(
                Math.Max(left.X, right.X),
                Math.Max(left.Y, right.Y),
                Math.Max(left.Z, right.Z),
                Math.Max(left.W, right.W));
        }

        /// <summary>
        ///  Calculates a vector with minimum X and Y component of two vectors.
        /// </summary>
        /// <param name="left">First vector.</param>
        /// <param name="right">Second vector.</param>
        /// <returns>A vector containing the minimum X and Y component.</returns>
        public static Vector4 Min(Vector4 left, Vector4 right)
        {
            return new Vector4(
                Math.Min(left.X, right.X),
                Math.Min(left.Y, right.Y),
                Math.Min(left.Z, right.Z),
                Math.Min(left.W, right.W));
        }

        /// <summary>
        ///  Negates a vector by mutliplying the X and Y by -1.0f.
        /// </summary>
        /// <param name="vector">The vector to negate.</param>
        /// <returns>The negated input vector.</returns>
        public static Vector4 Negate(Vector4 vector)
        {
            return new Vector4(-vector.X, -vector.Y, -vector.Z, -vector.W);
        }

        /// <summary>
        ///  Normalizes a vector.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized input vector.</returns>
        public static Vector4 Normalize(Vector4 vector)
        {
            Vector4 r = new Vector4(vector.X, vector.Y, vector.Z, vector.W);
            r.Normalize();

            return r;
        }
    }
}
