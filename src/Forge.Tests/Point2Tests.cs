using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Tests
{
    [TestClass]
    public class Point2Tests
    {
        [TestMethod]
        public void Construct_With_X_Y_Parameters_Sets_X_Y_Properties()
        {
            var a = new Point2(5, 7);
            Assert.AreEqual(5, a.X);
            Assert.AreEqual(7, a.Y);

            var b = new Point2(-3, 6);
            Assert.AreEqual(-3, b.X);
            Assert.AreEqual(6, b.Y);
        }

        [TestMethod]
        public void Zero_Static_Property_Has_Zero_For_X_Y()
        {
            var a = Point2.Zero;
            Assert.AreEqual(0, a.X);
            Assert.AreEqual(0, a.Y);
        }

        [TestMethod]
        public void Can_Get_And_Set_X_Property()
        {
            var a = Point2.Zero;
            Assert.AreEqual(0, a.X);

            a.X = 42;
            Assert.AreEqual(42, a.X);

            a.X = 5;
            Assert.AreEqual(5, a.X);
        }

        [TestMethod]
        public void Can_Get_And_Set_Y_Property()
        {
            var a = Point2.Zero;
            Assert.AreEqual(0, a.Y);

            a.Y = 42;
            Assert.AreEqual(42, a.Y);

            a.Y = 5;
            Assert.AreEqual(5, a.Y);
        }

        [TestMethod]
        public void Is_Zero_Is_True_Only_When_X_And_Y_Are_Zero()
        {
            var a = new Point2(2, 3);
            Assert.IsFalse(a.IsZero);

            a.X = 0;
            Assert.IsFalse(a.IsZero);

            a.Y = 0;
            Assert.IsTrue(a.IsZero);

            a.X = 2;
            Assert.IsFalse(a.IsZero);
        }

        [TestMethod]
        public void Compare_To_Is_Zero_When_Points_Equal()
        {
            var a = new Point2(4, 1);
            var b = new Point2(4, 1);

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.AreEqual(0, ((IComparable)a).CompareTo(b));
        }

        [TestMethod]
        public void Compare_To_Sorts_On_Y_If_Y_Values_Different()
        {
            var a = new Point2(3, 1);

            Assert.AreEqual(1, a.CompareTo(new Point2(-2, 1)));
            Assert.AreEqual(1, ((IComparable)a).CompareTo(new Point2(-2, 1)));

            Assert.AreEqual(-1, a.CompareTo(new Point2(4, 1)));
            Assert.AreEqual(-1, ((IComparable)a).CompareTo(new Point2(4, 1)));
        }

        [TestMethod]
        public void Compare_To_Sorts_On_X_If_Y_Values_Different()
        {
            var a = new Point2(3, 1);

            Assert.AreEqual(1, a.CompareTo(new Point2(3, 0)));
            Assert.AreEqual(1, ((IComparable)a).CompareTo(new Point2(3, 0)));

            Assert.AreEqual(-1, a.CompareTo(new Point2(3, 5)));
            Assert.AreEqual(-1, ((IComparable)a).CompareTo(new Point2(3, 5)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Compare_To_Throws_Exception_If_Other_Object_Is_Different_Type()
        {
            var a = new Point2(0, 0);
            Assert.AreEqual(1, ((IComparable)a).CompareTo(new object()));
        }

        [TestMethod]
        public void Equality_Checks_Both_X_Y()
        {
            var a = new Point2(1, 2);
            var a1 = new Point2(1, 2);
            var b = new Point2(1, 0);
            var c = new Point2(0, 2);
            var d = new Point2(0, 0);

            Assert.IsTrue(a == a1);
            Assert.IsFalse(a != a1);
            Assert.IsTrue(a.Equals(a1));
            Assert.IsTrue(a.Equals((object)a1));

            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object)b));

            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);
            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(a.Equals((object)c));

            Assert.IsFalse(a == d);
            Assert.IsTrue(a != d);
            Assert.IsFalse(a.Equals(d));
            Assert.IsFalse(a.Equals((object)d));
        }

        [TestMethod]
        public void Equals_Throws_Exception_If_Other_Object_Is_Different_Type()
        {
            var a = new Point2(0, 0);
            Assert.IsFalse(((object)a).Equals(new object()));
        }

        [TestMethod]
        public void Same_Point_Values_Have_Same_Hash_Code()
        {
            var a = new Point2(5, 8);
            var b = new Point2(5, 8);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Point_With_Transposed_Components_Have_Different_Hash_Codes()
        {
            var a = new Point2(8, 5);
            var b = new Point2(5, 8);

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Different_Point_Values_Have_Different_Hash_Codes()
        {
            var a = new Point2(2, 5);
            var b = new Point2(3, 4);

            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void Convert_Point_To_String_Returns_Simple_Readable_Text()
        {
            var a = new Point2(3, 5);
            var b = new Point2(-2, 1);

            Assert.AreEqual("3, 5", a.ToString());
            Assert.AreEqual("-2, 1", b.ToString());
        }

        [TestMethod]
        public void Negate_Will_Negate_Each_Component_Of_A_Point()
        {
            var a = new Point2(3, 5);
            var b = new Point2(-2, 1);
            
            Assert.AreEqual(new Point2(-3, -5), Point2.Negate(a));
            Assert.AreEqual(new Point2(2, -1), Point2.Negate(b));
        }

        [TestMethod]
        public void Unary_Positive_Operator_Does_Not_Change_Value()
        {
            var a = new Point2(3, 5);
            var b = new Point2(-2, 1);

            Assert.AreEqual(new Point2(3, 5), +a);
            Assert.AreEqual(new Point2(-2, 1), +b);
        }

        [TestMethod]
        public void Unary_Negative_Inverts_X_And_Y()
        {
            var a = new Point2(3, 5);
            var b = new Point2(-2, 1);

            Assert.AreEqual(new Point2(-3, -5), -a);
            Assert.AreEqual(new Point2(2, -1), -b);
        }

        [TestMethod]
        public void Add_Sums_Each_Component_Of_A_Point()
        {
            var a0 = new Point2(3, 5);
            var b0 = new Point2(-2, 1);
            var r0 = new Point2(1, 6);

            var a1 = new Point2(6, 2);
            var b1 = new Point2(0, 1);
            var r1 = new Point2(6, 3);

            Assert.AreEqual(r0, a0 + b0);
            Assert.AreEqual(r0, Point2.Add(a0, b0));

            Assert.AreEqual(r1, a1 + b1);
            Assert.AreEqual(r1, Point2.Add(a1, b1));
        }

        [TestMethod]
        public void Subtract_Subtracts_Each_Component_Of_A_Point()
        {
            var a0 = new Point2(3, 5);
            var b0 = new Point2(-2, 1);
            var r0 = new Point2(5, 4);

            var a1 = new Point2(6, 2);
            var b1 = new Point2(0, 1);
            var r1 = new Point2(6, 1);

            Assert.AreEqual(r0, a0 - b0);
            Assert.AreEqual(r0, Point2.Subtract(a0, b0));

            Assert.AreEqual(r1, a1 - b1);
            Assert.AreEqual(r1, Point2.Subtract(a1, b1));
        }

        [TestMethod]
        public void Multiply_Multiplies_Each_Component_Of_A_Point()
        {
            var a0 = new Point2(3, 5);
            var b0 = -2;
            var r0 = new Point2(-6, -10);

            var a1 = new Point2(6, 2);
            var b1 = 3;
            var r1 = new Point2(18, 6);

            Assert.AreEqual(r0, a0 * b0);
            Assert.AreEqual(r0, Point2.Multiply(a0, b0));

            Assert.AreEqual(r1, a1 * b1);
            Assert.AreEqual(r1, Point2.Multiply(a1, b1));
        }

        [TestMethod]
        public void Divide_Divides_Each_Component_Of_A_Point()
        {
            var a0 = new Point2(3, 5);
            var b0 = -1;
            var r0 = new Point2(-3, -5);

            var a1 = new Point2(6, 2);
            var b1 = 2;
            var r1 = new Point2(3, 1);

            Assert.AreEqual(r0, a0 / b0);
            Assert.AreEqual(r0, Point2.Divide(a0, b0));

            Assert.AreEqual(r1, a1 / b1);
            Assert.AreEqual(r1, Point2.Divide(a1, b1));
        }

        [TestMethod]
        public void Distance_Returns_Euclidean_Distance_Between_Two_Points()
        {
            var a = new Point2(0, 0);
            var b = new Point2[] { new Point2(0, 4), new Point2(4, 0), new Point2(-4, 0), new Point2(3, 4) };
            var r = new int[] { 4, 4, 4, 5 };

            Assert.AreEqual(r[0], Point2.Distance(a, b[0]));
            Assert.AreEqual(r[1], Point2.Distance(a, b[1]));
            Assert.AreEqual(r[2], Point2.Distance(a, b[2]));
            Assert.AreEqual(r[3], Point2.Distance(a, b[3]));
        }

        [TestMethod]
        public void Distance_Squared_Returns_Euclidean_Distance_Between_Two_Points_Without_Square_Root()
        {
            var a = new Point2(0, 0);
            var b = new Point2[] { new Point2(0, 4), new Point2(4, 0), new Point2(-4, 0), new Point2(3, 4) };
            var r = new int[] { 16, 16, 16, 25 };

            Assert.AreEqual(r[0], Point2.DistanceSquared(a, b[0]));
            Assert.AreEqual(r[1], Point2.DistanceSquared(a, b[1]));
            Assert.AreEqual(r[2], Point2.DistanceSquared(a, b[2]));
            Assert.AreEqual(r[3], Point2.DistanceSquared(a, b[3]));
        }

        [TestMethod]
        public void Min_Takes_Minimum_Values_For_Each_Component()
        {
            var a = new Point2(2, 3);
            var b = new Point2(1, 5);
            var c = new Point2(0, 2);

            Assert.AreEqual(new Point2(1, 3), Point2.Min(a, b));
            Assert.AreEqual(new Point2(0, 2), Point2.Min(a, c));
        }

        [TestMethod]
        public void Point_Max_Takes_Maximum_Values_For_Each_Component()
        {
            var a = new Point2(2, 3);
            var b = new Point2(1, 5);
            var c = new Point2(0, 2);

            Assert.AreEqual(new Point2(2, 5), Point2.Max(a, b));
            Assert.AreEqual(new Point2(2, 3), Point2.Max(a, c));
        }
    }
}
