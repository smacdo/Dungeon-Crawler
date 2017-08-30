using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SizeFTests
    {
        [TestMethod]
        [TestCategory("Forge/Size")]
        public void ValueConstructorSetsProperties()
        {
            var a = new SizeF(1.0f, 2.0f);

            Assert.AreEqual(1.0f, a.Width);
            Assert.AreEqual(2.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeWidthInConstructorThrowsException()
        {
            var a = new SizeF(-1.0f, 2.0f);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeHeightInConstructorThrowsException()
        {
            var a = new SizeF(1.0f, -1.0f);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void VectorConstructorSetsProperties()
        {
            var a = new SizeF(new Vector2(1.0f, 2.0f));

            Assert.AreEqual(1.0f, a.Width);
            Assert.AreEqual(2.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeWidthInVectorConstructorThrowsException()
        {
            var a = new SizeF(new Vector2(-1.0f, 2.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeHeightInVectorConstructorThrowsException()
        {
            var a = new SizeF(new Vector2(1.0f, -1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void SizeCopyConstructor()
        {
            var a = new SizeF(new SizeF(1.0f, 2.0f));

            Assert.AreEqual(1.0f, a.Width);
            Assert.AreEqual(2.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void CanSetWidthAndHeight()
        {
            var a = new SizeF(1.0f, 2.0f);

            a.Width = 5.0f;
            Assert.AreEqual(5.0f, a.Width);

            a.Height = 8.0f;
            Assert.AreEqual(8.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetWidthNegativeThrowsException()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Width = -1.0f;
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetHeightNegativeThrowsException()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Height = -1.0f;
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void IsEmptyTrueForZeroWidthOrHeight()
        {
            var a = new SizeF(1.0f, 2.0f);
            var b = new SizeF(0.0f, 0.0f);
            var c = new SizeF(0.0f, 1.0f);
            var d = new SizeF(1.0f, 0.0f);

            Assert.IsFalse(a.IsEmpty);
            Assert.IsTrue(b.IsEmpty);
            Assert.IsTrue(c.IsEmpty);
            Assert.IsTrue(d.IsEmpty);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void ScaleIgnoreAspectRatio()
        {
            var a = new SizeF(1.0f, 2.0f);
            var expected = new SizeF(7.0f, 3.0f);

            Assert.AreEqual(expected, a.Scale(new SizeF(7.0f, 3.0f)));
            Assert.AreEqual(expected, a.Scale(7.0f, 3.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void ScaleToZeroWidthThrowsException1()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Scale(0.0f, 1.0f);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void ScaleToZeroWidthThrowsException2()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Scale(new SizeF(0.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void ScaleToZeroHeightThrowsException1()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Scale(1.0f, 0.0f);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        [ExpectedException(typeof(ArgumentException))]
        public void ScaleToZeroHeightThrowsException2()
        {
            var a = new SizeF(1.0f, 2.0f);
            a.Scale(new SizeF(1.0f, 0.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void Comparing()
        {
            var a = new SizeF(4.0f, 6.0f);
            var b = new SizeF(6.0f, 4.0f);
            var c = new SizeF(5.0f, 5.0f);
            var d = new SizeF(7.0f, 1.0f);
            var e = new SizeF(0.0f, 0.0f);

            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(0, ((IComparable) a).CompareTo(a));

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.AreEqual(0, ((IComparable) a).CompareTo(b));
            Assert.IsFalse(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsTrue(a >= b);

            Assert.AreEqual(-1, a.CompareTo(c));
            Assert.AreEqual(-1, ((IComparable) a).CompareTo(c));
            Assert.IsTrue(a < c);
            Assert.IsTrue(a <= c);
            Assert.IsFalse(a > c);
            Assert.IsFalse(a >= c);

            Assert.AreEqual(1, a.CompareTo(d));
            Assert.AreEqual(1, ((IComparable) a).CompareTo(d));
            Assert.IsFalse(a < d);
            Assert.IsFalse(a <= d);
            Assert.IsTrue(a > d);
            Assert.IsTrue(a >= d);

            Assert.AreEqual(1, a.CompareTo(e));
            Assert.AreEqual(1, ((IComparable) a).CompareTo(e));
            Assert.IsFalse(a < e);
            Assert.IsFalse(a <= e);
            Assert.IsTrue(a > e);
            Assert.IsTrue(a >= e);
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void Transposed()
        {
            var a = new SizeF(2.0f, 3.0f);
            var expected = new SizeF(3.0f, 2.0f);

            Assert.AreEqual(expected, a.Tranposed());
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void ToVector2()
        {
            var a = new SizeF(2.0f, 3.0f);
            var expected = new Vector2(2.0f, 3.0f);

            Assert.AreEqual(expected, a.ToVector2());
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void HashCode()
        {
            var a = new SizeF(2.0f, 3.0f);
            var b = new SizeF(2.0f, 3.0f);
            var c = new SizeF(4.0f, 3.0f);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void ToStringDefault()
        {
            var a = new SizeF(2.0f, 3.5f);
            var s = "<w: 2, h: 3.5>";

            Assert.AreEqual(s, a.ToString());
        }

        [TestMethod]
        [TestCategory("Forge/Size")]
        public void Equals()
        {
            var a = new SizeF(2.0f, 3.0f);
            var b = new SizeF(2.0f, 3.0f);
            var c = new SizeF(2.0f, 1.0f);
            var d = new SizeF(1.0f, 3.0f);
            var e = new SizeF(1.0f, 4.0f);

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object) a));

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object) b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);

            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(a.Equals((object) c));
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);

            Assert.IsFalse(a.Equals(d));
            Assert.IsFalse(a.Equals((object) d));
            Assert.IsFalse(a == d);
            Assert.IsTrue(a != d);

            Assert.IsFalse(a.Equals(e));
            Assert.IsFalse(a.Equals((object) e));
            Assert.IsFalse(a == e);
            Assert.IsTrue(a != e);
        }
    }
}
