using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scott.Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Vector2Tests
    {
        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void ValueConstructorSetsProperties()
        {
            var a = new Vector2(1.0f, 2.0f);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void CopyConstructorSetsProperties()
        {
            var b = new Vector2(1.0f, 2.0f);
            var a = new Vector2(b);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void ZeroVectorPropertyIsZero()
        {
            var zero = new Vector2(0.0f, 0.0f);
            Assert.AreEqual(zero, Vector2.Zero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void LengthSquared()
        {
            var a = new Vector2(3.0f, 4.0f);
            var expected = 25.0f;

            Assert.AreEqual(expected, a.LengthSquared);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Length()
        {
            var a = new Vector2(3.0f, 4.0f);
            var expected = 5.0f;

            Assert.AreEqual(expected, a.Length);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void IsZero()
        {
            var a = new Vector2(0.0f, 0.0f);
            var b = new Vector2(2.0f, 3.0f);
            var c = new Vector2(2.0f, 0.0f);
            var d = new Vector2(0.0f, 3.0f);

            Assert.IsTrue(a.IsZero);
            Assert.IsFalse(b.IsZero);
            Assert.IsFalse(c.IsZero);
            Assert.IsFalse(d.IsZero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Negated()
        {
            var a = new Vector2(1.0f, -2.0f);
            var expected = new Vector2(-1.0f, 2.0f);

            Assert.AreEqual(expected, a.Negated());
            Assert.AreEqual(Vector2.Zero, Vector2.Zero.Negated());
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Normalize()
        {
            var a = new Vector2(2.0f, 3.0f);
            var expected = new Vector2(
                a.X / a.Length,
                a.Y / a.Length);

            var result = a;
            result.Normalize();

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Normalized()
        {
            var a = new Vector2(2.0f, 3.0f);
            var expected = new Vector2(
                a.X / a.Length,
                a.Y / a.Length);

            Assert.AreEqual(expected, a.Normalized());
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void NormalizeStatic()
        {
            var a = new Vector2(2.0f, 3.0f);
            var expected = new Vector2(
                a.X / a.Length,
                a.Y / a.Length);

            Assert.AreEqual(expected, Vector2.Normalize(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizeVectorOfZeroLength()
        {
            var a = Vector2.Zero;
            a.Normalize();
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizedVectorOfZeroLength()
        {
            var a = Vector2.Zero;
            a.Normalized();
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void DefaultToString()
        {
            var a = new Vector2(2.5f, 3.0f);
            var s = "<x: 2.5, y: 3>";

            Assert.AreEqual(s, a.ToString());
            Assert.AreEqual(s, a.ToString(null));
            Assert.AreEqual(s, a.ToString(null, null));
            Assert.AreEqual(s, a.ToString(string.Empty));
            Assert.AreEqual(s, a.ToString(string.Empty, null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void ToStringFullFormat()
        {
            var a = new Vector2(2.5f, 3.0f);
            var s = "<Vector2 x: 2.5, y: 3>";

            Assert.AreEqual(s, a.ToString("F"));
            Assert.AreEqual(s, a.ToString("F", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void ToStringJsonFormat()
        {
            var a = new Vector2(2.5f, 3.0f);
            var s = "{ \"x\": \"2.5\", \"y\": \"3\" }";

            Assert.AreEqual(s, a.ToString("J"));
            Assert.AreEqual(s, a.ToString("J", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void ToStringValueFormat()
        {
            var a = new Vector2(2.5f, 3.0f);
            var s = "2.5, 3";

            Assert.AreEqual(s, a.ToString("V"));
            Assert.AreEqual(s, a.ToString("V", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException1()
        {
            var a = new Vector2(2.5f, 3.0f);
            a.ToString("LOL");
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException2()
        {
            var a = new Vector2(2.5f, 3.0f);
            a.ToString("LOL", null);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Equals()
        {
            var a = new Vector2(2.0f, 3.0f);
            var b = new Vector2(2.0f, 3.0f);
            var c = new Vector2(2.0f, 0.0f);
            var d = new Vector2(0.0f, 3.0f);
            var e = new Vector2(0.0f, 0.0f);

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
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void TestGetHashCode()
        {
            // Simple unit test to check that we're getting some kind of result.
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(a);
            var c = new Vector2(3.0f, 1.0f);

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Negation()
        {
            var a = new Vector2(1.0f, -2.0f);
            var expected = new Vector2(-1.0f, 2.0f);

            Assert.AreEqual(expected, -a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void NegateStatic()
        {
            var a = new Vector2(1.0f, -2.0f);
            var expected = new Vector2(-1.0f, 2.0f);

            Assert.AreEqual(expected, Vector2.Negate(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Positation()
        {
            var a = new Vector2(1.0f, -2.0f);
            var expected = new Vector2(1.0f, -2.0f);

            Assert.AreEqual(expected, +a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Addition()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.5f, -1.0f);
            var expected = new Vector2(3.5f, 1.0f);

            Assert.AreEqual(expected, Vector2.Add(a, b));
            Assert.AreEqual(expected, a + b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Subtraction()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.5f, -1.0f);
            var expected = new Vector2(-1.5f, 3.0f);

            Assert.AreEqual(expected, Vector2.Subtract(a, b));
            Assert.AreEqual(expected, a - b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Multiplication()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = -3.0f;
            var expected = new Vector2(-3.0f, -6.0f);

            Assert.AreEqual(expected, Vector2.Multiply(a, b));
            Assert.AreEqual(expected, a * b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Division()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = -2.0f;
            var expected = new Vector2(-0.5f, -1.0f);

            Assert.AreEqual(expected, Vector2.Divide(a, b));
            Assert.AreEqual(expected, a / b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Cross()
        {
            var a = new Vector2(1.0f, 2.0f);
            Vector2.Cross(a, a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Distance()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.0f, -1.0f);
            var expected = (float)Math.Sqrt(
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f));

            Assert.AreEqual(expected, Vector2.Distance(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void DistanceSquared()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.0f, -1.0f);
            var expected =
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f);

            Assert.AreEqual(expected, Vector2.DistanceSquared(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void DotProduct()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.0f, -1.0f);
            var expected = 1.0f * 2.0f + 2.0f * -1.0f;

            Assert.AreEqual(expected, Vector2.Dot(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Max()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.0f, -1.0f);
            var expected = new Vector2(2.0f, 2.0f);

            Assert.AreEqual(expected, Vector2.Max(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector2")]
        public void Min()
        {
            var a = new Vector2(1.0f, 2.0f);
            var b = new Vector2(2.0f, -1.0f);
            var expected = new Vector2(1.0f, -1.0f);

            Assert.AreEqual(expected, Vector2.Min(a, b));
        }
    }
}
