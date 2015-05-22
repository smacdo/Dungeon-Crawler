using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class Vector3Tests
    {
        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void ValueConstructorSetsProperties()
        {
            var a = new Vector3(1.0f, 2.0f, 3.0f);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
            Assert.AreEqual(3.0f, a.Z);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void CopyConstructorSetsProperties()
        {
            var b = new Vector3(1.0f, 2.0f, 3.0f);
            var a = new Vector3(b);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
            Assert.AreEqual(3.0f, a.Z);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void ZeroVectorPropertyIsZero()
        {
            var zero = new Vector3(0.0f, 0.0f, 0.0f);
            Assert.AreEqual(zero, Vector3.Zero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void LengthSquared()
        {
            var a = new Vector3(1.0f, 2.0f, 2.0f);
            var expected = 9.0f;

            Assert.AreEqual(expected, a.LengthSquared);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Length()
        {
            var a = new Vector3(1.0f, 2.0f, 2.0f);
            var expected = 3.0f;

            Assert.AreEqual(expected, a.Length);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void IsZero()
        {
            var a = new Vector3(0.0f, 0.0f, 0.0f);
            var b = new Vector3(2.0f, 3.0f, 5.0f);
            var c = new Vector3(2.0f, 0.0f, 0.0f);
            var d = new Vector3(0.0f, 3.0f, 0.0f);
            var e = new Vector3(0.0f, 0.0f, 5.0f);

            Assert.IsTrue(a.IsZero);
            Assert.IsFalse(b.IsZero);
            Assert.IsFalse(c.IsZero);
            Assert.IsFalse(d.IsZero);
            Assert.IsFalse(e.IsZero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Negated()
        {
            var a = new Vector3(1.0f, -2.0f, -3.0f);
            var expected = new Vector3(-1.0f, 2.0f, 3.0f);

            Assert.AreEqual(expected, a.Negated());
            Assert.AreEqual(Vector3.Zero, Vector3.Zero.Negated());
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Normalize()
        {
            var a = new Vector3(2.0f, 3.0f, 5.0f);
            var expected = new Vector3(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length);

            var result = a;
            result.Normalize();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Normalized()
        {
            var a = new Vector3(2.0f, 3.0f, 5.0f);
            var expected = new Vector3(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length);

            Assert.AreEqual(expected, a.Normalized());
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void NormalizeStatic()
        {
            var a = new Vector3(2.0f, 3.0f, 5.0f);
            var expected = new Vector3(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length);

            Assert.AreEqual(expected, Vector3.Normalize(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizeVectorOfZeroLength()
        {
            var a = Vector3.Zero;
            a.Normalize();
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizedVectorOfZeroLength()
        {
            var a = Vector3.Zero;
            a.Normalized();
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void DefaultToString()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            var s = "<x: 2.5, y: 3, z: 5>";

            Assert.AreEqual(s, a.ToString());
            Assert.AreEqual(s, a.ToString(null));
            Assert.AreEqual(s, a.ToString(null, null));
            Assert.AreEqual(s, a.ToString(string.Empty));
            Assert.AreEqual(s, a.ToString(string.Empty, null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void ToStringFullFormat()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            var s = "<Vector3 x: 2.5, y: 3, z: 5>";

            Assert.AreEqual(s, a.ToString("F"));
            Assert.AreEqual(s, a.ToString("F", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void ToStringJsonFormat()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            var s = "{ \"x\": \"2.5\", \"y\": \"3\", \"z\": \"5\" }";

            Assert.AreEqual(s, a.ToString("J"));
            Assert.AreEqual(s, a.ToString("J", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void ToStringValueFormat()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            var s = "2.5, 3, 5";

            Assert.AreEqual(s, a.ToString("V"));
            Assert.AreEqual(s, a.ToString("V", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException1()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            a.ToString("LOL");
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException2()
        {
            var a = new Vector3(2.5f, 3.0f, 5.0f);
            a.ToString("LOL", null);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Equals()
        {
            var a = new Vector3(2.0f, 3.0f, 5.0f);
            var b = new Vector3(2.0f, 3.0f, 5.0f);
            var c = new Vector3(2.0f, 0.0f, 0.0f);
            var d = new Vector3(0.0f, 3.0f, 0.0f);
            var e = new Vector3(0.0f, 0.0f, 5.0f);
            var f = new Vector3(2.0f, 0.0f, 0.0f);
            var g = new Vector3(2.0f, 3.0f, 0.0f);

            Assert.IsFalse(a.Equals(null));

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object) a));

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object) b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);

            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(b.Equals((object) a));
            Assert.IsTrue(b == a);
            Assert.IsFalse(b != a);

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

            Assert.IsFalse(a.Equals(f));
            Assert.IsFalse(a.Equals((object) f));
            Assert.IsFalse(a == f);
            Assert.IsTrue(a != f);

            Assert.IsFalse(a.Equals(g));
            Assert.IsFalse(a.Equals((object) g));
            Assert.IsFalse(a == g);
            Assert.IsTrue(a != g);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void TestGetHashCode()
        {
            // Simple unit test to check that we're getting some kind of result.
            var a = new Vector3(1.0f, 2.0f, 3.0f);
            var b = new Vector3(a);
            var c = new Vector3(3.0f, 1.0f, 4.0f);

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Negation()
        {
            var a = new Vector3(1.0f, -2.0f, 3.0f);
            var expected = new Vector3(-1.0f, 2.0f, -3.0f);

            Assert.AreEqual(expected, -a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void NegateStatic()
        {
            var a = new Vector3(1.0f, -2.0f, 3.0f);
            var expected = new Vector3(-1.0f, 2.0f, -3.0f);

            Assert.AreEqual(expected, Vector3.Negate(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Positation()
        {
            var a = new Vector3(1.0f, -2.0f, 3.0f);
            var expected = new Vector3(1.0f, -2.0f, 3.0f);

            Assert.AreEqual(expected, +a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Addition()
        {
            var a = new Vector3(1.0f, 2.0f, 3.0f);
            var b = new Vector3(2.5f, -1.0f, 4.0f);
            var expected = new Vector3(3.5f, 1.0f, 7.0f);

            Assert.AreEqual(expected, Vector3.Add(a, b));
            Assert.AreEqual(expected, a + b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Subtraction()
        {
            var a = new Vector3(1.0f, 2.0f, 3.0f);
            var b = new Vector3(2.5f, -1.0f, 4.0f);
            var expected = new Vector3(-1.5f, 3.0f, -1.0f);

            Assert.AreEqual(expected, Vector3.Subtract(a, b));
            Assert.AreEqual(expected, a - b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Multiplication()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = -3.0f;
            var expected = new Vector3(-3.0f, -6.0f, 9.0f);

            Assert.AreEqual(expected, Vector3.Multiply(a, b));
            Assert.AreEqual(expected, a * b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Division()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = -2.0f;
            var expected = new Vector3(-0.5f, -1.0f, 1.5f);

            Assert.AreEqual(expected, Vector3.Divide(a, b));
            Assert.AreEqual(expected, a / b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Cross()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected = new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

            Assert.AreEqual(expected, Vector3.Cross(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Distance()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected = (float)Math.Sqrt(
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f) +
                (0.5f - -3.0f) * (0.5f - -3.0f));

            Assert.AreEqual(expected, Vector3.Distance(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void DistanceSquared()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected =
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f) +
                (0.5f - -3.0f) * (0.5f - -3.0f);

            Assert.AreEqual(expected, Vector3.DistanceSquared(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void DotProduct()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected = 1.0f * 2.0f + 2.0f * -1.0f + -3.0f * 0.5f;

            Assert.AreEqual(expected, Vector3.Dot(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Max()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected = new Vector3(2.0f, 2.0f, 0.5f);

            Assert.AreEqual(expected, Vector3.Max(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector3")]
        public void Min()
        {
            var a = new Vector3(1.0f, 2.0f, -3.0f);
            var b = new Vector3(2.0f, -1.0f, 0.5f);
            var expected = new Vector3(1.0f, -1.0f, -3.0f);

            Assert.AreEqual(expected, Vector3.Min(a, b));
        }
    }
}
