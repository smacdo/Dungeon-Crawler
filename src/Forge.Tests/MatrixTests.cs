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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Tests
{
    [TestClass]
    public class Matrix3Tests
    {
        [TestMethod]
        public void Constructor_Takes_Argument_In_Row_Major_Order()
        {
            var a = new Matrix3(10, 20, 30, 40, 50, 60, 70, 80, 90);

            Assert.AreEqual(10, a.m11);
            Assert.AreEqual(20, a.m12);
            Assert.AreEqual(30, a.m13);

            Assert.AreEqual(40, a.m21);
            Assert.AreEqual(50, a.m22);
            Assert.AreEqual(60, a.m23);

            Assert.AreEqual(70, a.m31);
            Assert.AreEqual(80, a.m32);
            Assert.AreEqual(90, a.m33);
        }

        [TestMethod]
        public void Copy_Constructor_Copies_All_Cells()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var b = new Matrix3(a);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void Row_Vector_Constructor_Copies_All_Cells()
        {
            var a = new Matrix3(
                new Vector3(11, 12, 13),
                new Vector3(14, 15, 16),
                new Vector3(17, 18, 19));
            var b = new Matrix3(11, 12, 13, 14, 15, 16, 17, 18, 19);

            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void Is_Identity_Is_Only_True_For_Identity_Matrix()
        {
            var a = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            var b = new Matrix3(0, 0, 0, 0, 1, 0, 0, 0, 1);
            var c = new Matrix3(1, 0, 0, 0, 0, 0, 0, 0, 1);
            var d = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 0);

            Assert.IsTrue(a.IsIdentity);
            Assert.IsFalse(b.IsIdentity);
            Assert.IsFalse(c.IsIdentity);
            Assert.IsFalse(d.IsIdentity);
        }

        [TestMethod]
        public void Is_Zero_Is_True_When_All_Cells_Are_Zero()
        {
            var a = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            Assert.IsTrue(a.IsZero);

            var c = new Matrix3[]
            {
                new Matrix3(1, 0, 0, 0, 0, 0, 0, 0, 0),
                new Matrix3(0, 1, 0, 0, 0, 0, 0, 0, 0),
                new Matrix3(0, 0, 1, 0, 0, 0, 0, 0, 0),
                new Matrix3(0, 0, 0, 1, 0, 0, 0, 0, 0),
                new Matrix3(0, 0, 0, 0, 1, 0, 0, 0, 0),
                new Matrix3(0, 0, 0, 0, 0, 1, 0, 0, 0),
                new Matrix3(0, 0, 0, 0, 0, 0, 1, 0, 0),
                new Matrix3(0, 0, 0, 0, 0, 0, 0, 1, 0),
                new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 1),
            };

            Assert.IsFalse(c.Any(x => x.IsZero));
        }

        [TestMethod]
        public void Identity_Property_Has_Diagonal_Cells_Set_To_One()
        {
            var a = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Assert.AreEqual(a, Matrix3.Identity);
        }

        [TestMethod]
        public void Zero_Property_Has_Cells_Set_To_Zero()
        {
            var a = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            Assert.AreEqual(a, Matrix3.Zero);
        }

        [TestMethod]
        public void Equals_Matches_All_Exactly()
        {
            var a = new Matrix3(1, 2, 2.0f/3, 4, 5, 6, 7, 8, 9);
            
            Assert.AreEqual(a, a);
            Assert.IsTrue(((object)a).Equals(a));
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(Matrix3.AreEqual(ref a, ref a));
            
            // All values off.
            var b = Matrix3.Zero;

            Assert.AreNotEqual(a, b);
            Assert.IsFalse(((object)a).Equals(b));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(Matrix3.AreEqual(ref a, ref b));

            // Test when only one is off.
            var c = new Matrix3[]
            {
                new Matrix3(0, 2, 3, 4, 5, 6, 7, 8, 9),
                new Matrix3(1, 0, 3, 4, 5, 6, 7, 8, 9),
                new Matrix3(1, 2, 0, 4, 5, 6, 7, 8, 9),
                new Matrix3(1, 2, 3, 0, 5, 6, 7, 8, 9),
                new Matrix3(1, 2, 3, 4, 0, 6, 7, 8, 9),
                new Matrix3(1, 2, 3, 4, 5, 0, 7, 8, 9),
                new Matrix3(1, 2, 3, 4, 5, 6, 0, 8, 9),
                new Matrix3(1, 2, 3, 4, 5, 6, 7, 0, 9),
                new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 0),
            };

            Assert.IsFalse(c.Any(x => ((object)a).Equals(x)));
        }

        [TestMethod]
        public void Hashcode_Is_Probably_Unique()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var b = new Matrix3(a);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), Matrix3.Zero.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), Matrix3.Identity.GetHashCode());
        }

        [TestMethod]
        public void Hashcode_Changes_When_Positions_Shift()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // Test when only one is off.
            var c = new Matrix3[]
            {
                new Matrix3(2, 1, 3, 4, 5, 6, 7, 8, 9),
                new Matrix3(1, 5, 3, 4, 2, 6, 7, 8, 9),
                new Matrix3(3, 2, 1, 4, 5, 6, 7, 8, 9),
                new Matrix3(1, 2, 3, 7, 5, 6, 4, 8, 9),
                new Matrix3(1, 2, 3, 4, 9, 6, 7, 8, 5),
                new Matrix3(1, 6, 3, 4, 5, 2, 7, 8, 9),
                new Matrix3(1, 2, 3, 4, 5, 6, 8, 7, 9),
                new Matrix3(8, 2, 3, 4, 5, 6, 7, 1, 9),
                new Matrix3(1, 2, 3, 4, 5, 9, 7, 8, 6),
            };

            Assert.IsFalse(c.Any(x => a.GetHashCode() == base.GetHashCode()));
        }

        [TestMethod]
        public void Cell_Access_Is_Row_Major()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(1, a[0]);
            Assert.AreEqual(2, a[1]);
            Assert.AreEqual(3, a[2]);

            Assert.AreEqual(4, a[3]);
            Assert.AreEqual(5, a[4]);
            Assert.AreEqual(6, a[5]);

            Assert.AreEqual(7, a[6]);
            Assert.AreEqual(8, a[7]);
            Assert.AreEqual(9, a[8]);
        }

        [TestMethod]
        public void Row_Column_Indexer_Returns_Correct_Cell()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(1, a[0, 0]);
            Assert.AreEqual(2, a[0, 1]);
            Assert.AreEqual(3, a[0, 2]);

            Assert.AreEqual(4, a[1, 0]);
            Assert.AreEqual(5, a[1, 1]);
            Assert.AreEqual(6, a[1, 2]);

            Assert.AreEqual(7, a[2, 0]);
            Assert.AreEqual(8, a[2, 1]);
            Assert.AreEqual(9, a[2, 2]);
        }

        [TestMethod]
        public void Point_Indexer_Returns_Correct_Cell()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(1, a[new Point2(0, 0)]);
            Assert.AreEqual(2, a[new Point2(1, 0)]);
            Assert.AreEqual(3, a[new Point2(2, 0)]);

            Assert.AreEqual(4, a[new Point2(0, 1)]);
            Assert.AreEqual(5, a[new Point2(1, 1)]);
            Assert.AreEqual(6, a[new Point2(2, 1)]);

            Assert.AreEqual(7, a[new Point2(0, 2)]);
            Assert.AreEqual(8, a[new Point2(1, 2)]);
            Assert.AreEqual(9, a[new Point2(2, 2)]);
        }

        [TestMethod]
        public void Get_Column_Vector_Returns_Column_From_Matrix()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(new Vector3(1, 4, 7), a.GetColumn(0));
            Assert.AreEqual(new Vector3(2, 5, 8), a.GetColumn(1));
            Assert.AreEqual(new Vector3(3, 6, 9), a.GetColumn(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Get_Column_Vector_Throws_Exception_For_Invalid_Column_Index()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            a.GetColumn(3);
        }

        [TestMethod]
        public void Get_Row_Vector_Returns_Row_From_Matrix()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            Assert.AreEqual(new Vector3(1, 2, 3), a.GetRow(0));
            Assert.AreEqual(new Vector3(4, 5, 6), a.GetRow(1));
            Assert.AreEqual(new Vector3(7, 8, 9), a.GetRow(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Get_Row_Vector_Throws_Exception_For_Invalid_Row_Index()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            a.GetRow(3);
        }

        [TestMethod]
        public void Tranpose_Correctly_Transposes_Values()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var expected = new Matrix3(1, 4, 7, 2, 5, 8, 3, 6, 9);

            // Check instance method.
            Assert.AreEqual(expected, a.Transposed());

            // Check static method.
            var result = Matrix3.Zero;
            Matrix3.Transpose(ref a, out result);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void To_String_Produces_Readable_Matrix()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.AreEqual("((1, 2, 3), (4, 5, 6), (7, 8, 9))", a.ToString());
        }

        [TestMethod]
        public void Negate_Multiplies_All_Cells_By_Negative_One()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var expected = new Matrix3(-1, -2, -3, -4, -5, -6, -7, -8, -9);

            // Check instance method.
            Assert.AreEqual(expected, a.Negated());

            // Check static method.
            var result = Matrix3.Zero;
            Matrix3.Negate(ref a, out result);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Add_Two_Matrices()
        {
            var a = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var b = new Matrix3(10, 20, 30, 40, 50, 60, 70, 80, 90);
            var expected = new Matrix3(11, 22, 33, 44, 55, 66, 77, 88, 99);

            // Check operator.
            Assert.AreEqual(expected, a + b);

            // Check static method.
            var result = Matrix3.Zero;
            Matrix3.Add(ref a, ref b, out result);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Subtact_Two_Matrices()
        {
            var a = new Matrix3(10, 20, 30, 40, 50, 60, 70, 80, 90);
            var b = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);
            var expected = new Matrix3(9, 18, 27, 36, 45, 54, 63, 72, 81);

            // Check operator.
            Assert.AreEqual(expected, a - b);

            // Check static method.
            var result = Matrix3.Zero;
            Matrix3.Subtract(ref a, ref b, out result);

            Assert.AreEqual(expected, result);
        }
    }
}