/*
 * Copyright 2012-2018 Scott MacDonald
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

namespace Forge
{
    /// <summary>
    ///  3x3 matrix with row major layout.
    /// </summary>
    /// <remarks>
    ///  Matrix stores values internally in row major format.
    ///  
    ///  [ [ m11 m12 m13 ]
    //     [ m21 m22 m23 ] 
    //     [ m31 m32 m33 ] ]
    /// </remarks>
    [DataContract]
    public struct Matrix3 : IEquatable<Matrix3>
    {
        [DataMember(Order = 0, IsRequired = true)] public float m11;
        [DataMember(Order = 1, IsRequired = true)] public float m12;
        [DataMember(Order = 2, IsRequired = true)] public float m13;

        [DataMember(Order = 3, IsRequired = true)] public float m21;
        [DataMember(Order = 4, IsRequired = true)] public float m22;
        [DataMember(Order = 5, IsRequired = true)] public float m23;

        [DataMember(Order = 6, IsRequired = true)] public float m31;
        [DataMember(Order = 7, IsRequired = true)] public float m32;
        [DataMember(Order = 8, IsRequired = true)] public float m33;

        /// <summary>
        ///  Constructor with parameters in row major order.
        /// </summary>
        /// <param name="m11">Row 1 column 1 value.</param>
        /// <param name="m12">Row 1 column 2 value.</param>
        /// <param name="m13">Row 1 column 3 value.</param>
        /// <param name="m21">Row 2 column 1 value.</param>
        /// <param name="m22">Row 2 column 2 value.</param>
        /// <param name="m23">Row 2 column 3 value.</param>
        /// <param name="m31">Row 3 column 1 value.</param>
        /// <param name="m32">Row 3 column 2 value.</param>
        /// <param name="m33">Row 3 column 3 value.</param>
        public Matrix3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;

            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;

            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        /// <summary>
        ///  Copy constructor.
        /// </summary>
        /// <param name="other">Matrix to copy values from.</param>
        public Matrix3(Matrix3 other)
        {
            m11 = other.m11;
            m12 = other.m12;
            m13 = other.m13;

            m21 = other.m21;
            m22 = other.m22;
            m23 = other.m23;

            m31 = other.m31;
            m32 = other.m32;
            m33 = other.m33;
        }

        /// <summary>
        ///  Row vector constructor.
        /// </summary>
        /// <param name="a">Row 1 value.</param>
        /// <param name="b">Row 2 value.</param>
        /// <param name="c">Row 3 value.</param>
        public Matrix3(Vector3 a, Vector3 b, Vector3 c)
        {
            m11 = a.X;
            m12 = a.Y;
            m13 = a.Z;

            m21 = b.X;
            m22 = b.Y;
            m23 = b.Z;

            m31 = c.X;
            m32 = c.Y;
            m33 = c.Z;
        }

        /// <summary>
        ///  Get if this is an identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get { return AreEqual(ref this, ref _identity); }
        }

        /// <summary>
        ///  Get if this is a zero matrix.
        /// </summary>
        public bool IsZero
        {
            get { return AreEqual(ref this, ref _zero); }
        }

        /// <summary>
        ///  Internal readonly identity maatrix.
        /// </summary>
        private static Matrix3 _identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        /// <summary>
        ///  Get an identity matrix.
        /// </summary>
        public static Matrix3 Identity { get { return _identity; } }

        /// <summary>
        ///  Internal readonly zero matrix.
        /// </summary>
        private static Matrix3 _zero = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        ///  Get a zero matrix.
        /// </summary>
        public static Matrix3 Zero { get { return _zero; } }

        /// <summary>
        ///  Get or set a value in the matrix.
        /// </summary>
        /// <param name="index">Row major index of cell.</param>
        /// <returns>Value in cell.</returns>
        public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return m11;

					case 1:
						return m12;

					case 2:
						return m13;

					case 3:
						return m21;

					case 4:
						return m22;

					case 5:
						return m23;

					case 6:
						return m31;

					case 7:
						return m32;

					case 8:
						return m33;

					default:
						throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
			{
				switch (index)
				{
					case 0:
						m11 = value;
                        break;

					case 1:
						m12 = value;
                        break;

					case 2:
						m13 = value;
                        break;

					case 3:
						m21 = value;
                        break;

					case 4:
						m22 = value;
                        break;

					case 5:
						m23 = value;
                        break;

					case 6:
						m31 = value;
                        break;

					case 7:
						m32 = value;
                        break;

					case 8:
						m33 = value;
                        break;

					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
		}

        /// <summary>
        ///  Get or set a value in the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
		public float this[int row, int col]
		{

            get { return this[row * 3 + col]; }
			set { this[row* 3 + col] = value; }
		}

        /// <summary>
        ///  Get or set a value in the matrix.
        /// </summary>
        /// <param name="index">Index of value.</param>
		public float this[Point2 index]
        {

            get { return this[index.Y * 3 + index.X]; }
            set { this[index.Y * 3 + index.X] = value; }
        }

        /// <summary>
        ///  Get a column vector from the matrix.
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column vector from the matrix.</returns>
        public Vector3 GetColumn(int column)
        {
            switch (column)
            {
                case 0:
                    return new Vector3(m11, m21, m31);

                case 1:
                    return new Vector3(m12, m22, m32);

                case 2:
                    return new Vector3(m13, m23, m33);

                default:
                    throw new ArgumentOutOfRangeException(nameof(column));
            }
        }

        /// <summary>
        ///  Get a row vector from the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>Row vector from the matrix.</returns>
        public Vector3 GetRow(int row)
        {
            switch (row)
            {
                case 0:
                    return new Vector3(m11, m12, m13);

                case 1:
                    return new Vector3(m21, m22, m23);

                case 2:
                    return new Vector3(m31, m32, m33);

                default:
                    throw new ArgumentOutOfRangeException(nameof(row));
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Matrix3 m)
            {
                return AreEqual(ref this, ref m);
            }

            return false;
        }

        /// <inheritdoc />
        public bool Equals(Matrix3 other)
        {
            return AreEqual(ref this, ref other);
        }

        /// <summary>
        ///  Check if two matrices are exactly equal.
        /// </summary>
        /// <param name="a">First matrix to check.</param>
        /// <param name="b">Second matrix to check.</param>
        /// <returns>True if the two matrices are exactly equal, false otherwise.</returns>
        public static bool AreEqual(ref Matrix3 a, ref Matrix3 b)
        {
            return
                a.m11 == b.m11 && a.m12 == b.m12 && a.m13 == b.m13 &&
                a.m21 == b.m21 && a.m22 == b.m22 && a.m23 == b.m23 &&
                a.m31 == b.m31 && a.m32 == b.m32 && a.m33 == b.m33;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 23) + m11.GetHashCode();
                hash = (hash * 23) + m12.GetHashCode();
                hash = (hash * 23) + m13.GetHashCode();

                hash = (hash * 23) + m21.GetHashCode();
                hash = (hash * 23) + m22.GetHashCode();
                hash = (hash * 23) + m23.GetHashCode();

                hash = (hash * 23) + m31.GetHashCode();
                hash = (hash * 23) + m32.GetHashCode();
                hash = (hash * 23) + m33.GetHashCode();

                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"(({m11}, {m12}, {m13}), ({m21}, {m22}, {m23}), ({m31}, {m32}, {m33}))";
        }

        public static float Determinant(ref Matrix3 a)
        {
            throw new NotImplementedException();
        }

        public static void Invert(ref Matrix3 a, out Matrix3 result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Get the transpose of this matrix.
        /// </summary>
        public Matrix3 Transposed()
        {
            return new Matrix3(m11, m21, m31, m12, m22, m32, m13, m23, m33);
        }

        /// <summary>
        ///  Calculate the transpose of a matrix.
        /// </summary>
        /// <param name="a">Matrix to transpose.</param>
        /// <param name="result">Matrix to receive the transposed result.</param>
        public static void Transpose(ref Matrix3 a, out Matrix3 result)
        {
            result.m11 = a.m11;
            result.m12 = a.m21;
            result.m13 = a.m31;

            result.m21 = a.m12;
            result.m22 = a.m22;
            result.m23 = a.m32;

            result.m31 = a.m13;
            result.m32 = a.m23;
            result.m33 = a.m33;
        }

        /// <summary>
        ///  Get the negation of a matrix (multiplying all values by negative one).
        /// </summary>
        public Matrix3 Negated()
        {
            return new Matrix3(-m11, -m12, -m13, -m21, -m22, -m23, -m31, -m32, -m33);
        }

        /// <summary>
        ///  Calculate the negation of a matrix (multiplying all values by negative one).
        /// </summary>
        /// <param name="a">Matrix to negate.</param>
        /// <param name="result">Matrix to receive the transposed result.</param>
        public static void Negate(ref Matrix3 a, out Matrix3 result)
        {
            result.m11 = -a.m11;
            result.m12 = -a.m12;
            result.m13 = -a.m13;

            result.m21 = -a.m21;
            result.m22 = -a.m22;
            result.m23 = -a.m23;

            result.m31 = -a.m31;
            result.m32 = -a.m32;
            result.m33 = -a.m33;
        }

        /// <summary>
        ///  Addition operator.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <returns>Matrix containing the result of adding the first and second matrix.</returns>
        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(
                a.m11 + b.m11, a.m12 + b.m12, a.m13 + b.m13,
                a.m21 + b.m21, a.m22 + b.m22, a.m23 + b.m23,
                a.m31 + b.m31, a.m32 + b.m32, a.m33 + b.m33);
        }

        /// <summary>
        ///  Add two matrices together.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <param name="result">Matrix that receives the result.</param>
        public static void Add(ref Matrix3 a, ref Matrix3 b, out Matrix3 result)
        {
            result.m11 = a.m11 + b.m11;
            result.m12 = a.m12 + b.m12;
            result.m13 = a.m13 + b.m13;

            result.m21 = a.m21 + b.m21;
            result.m22 = a.m22 + b.m22;
            result.m23 = a.m23 + b.m23;

            result.m31 = a.m31 + b.m31;
            result.m32 = a.m32 + b.m32;
            result.m33 = a.m33 + b.m33;
        }

        /// <summary>
        ///  Subtraction operator.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <param name="result">Matrix that receives the result.</param>
        public static Matrix3 operator -(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(
                a.m11 - b.m11, a.m12 - b.m12, a.m13 - b.m13,
                a.m21 - b.m21, a.m22 - b.m22, a.m23 - b.m23,
                a.m31 - b.m31, a.m32 - b.m32, a.m33 - b.m33);
        }

        /// <summary>
        ///  Subtracts two matrices.
        /// </summary>
        /// <param name="a">First matrix.</param>
        /// <param name="b">Second matrix.</param>
        /// <param name="result">Matrix that receives the result.</param>
        public static void Subtract(ref Matrix3 a, ref Matrix3 b, out Matrix3 result)
        {
            result.m11 = a.m11 - b.m11;
            result.m12 = a.m12 - b.m12;
            result.m13 = a.m13 - b.m13;

            result.m21 = a.m21 - b.m21;
            result.m22 = a.m22 - b.m22;
            result.m23 = a.m23 - b.m23;

            result.m31 = a.m31 - b.m31;
            result.m32 = a.m32 - b.m32;
            result.m33 = a.m33 - b.m33;
        }

        public static void Multiply(ref Matrix3 a, ref Matrix3 b, out Matrix3 result)
        {
            result.m11 = (a.m11 * b.m11) + (a.m12 * b.m21) + (a.m13 * b.m31);
            result.m12 = (a.m11 * b.m12) + (a.m12 * b.m22) + (a.m13 * b.m32);
            result.m13 = (a.m11 * b.m13) + (a.m12 * b.m23) + (a.m13 * b.m33);

            result.m21 = (a.m21 * b.m11) + (a.m22 * b.m21) + (a.m23 * b.m31);
            result.m22 = (a.m21 * b.m12) + (a.m22 * b.m22) + (a.m23 * b.m32);
            result.m23 = (a.m21 * b.m13) + (a.m22 * b.m23) + (a.m23 * b.m33);

            result.m31 = (a.m31 * b.m11) + (a.m32 * b.m21) + (a.m33 * b.m31);
            result.m32 = (a.m31 * b.m12) + (a.m32 * b.m22) + (a.m33 * b.m32);
            result.m33 = (a.m31 * b.m13) + (a.m32 * b.m23) + (a.m33 * b.m33);
        }
	}
}
