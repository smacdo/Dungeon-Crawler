using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge;

namespace Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Vector4Tests
    {
        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void ValueConstructorSetsProperties()
        {
            var a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
            Assert.AreEqual(3.0f, a.Z);
            Assert.AreEqual(4.0f, a.W);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void CopyConstructorSetsProperties()
        {
            var b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            var a = new Vector4(b);

            Assert.AreEqual(1.0f, a.X);
            Assert.AreEqual(2.0f, a.Y);
            Assert.AreEqual(3.0f, a.Z);
            Assert.AreEqual(4.0f, a.W);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void ZeroVectorPropertyIsZero()
        {
            var zero = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(zero, Vector4.Zero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void LengthSquared()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var len = Math.Pow(2.0f, 2.0f) + Math.Pow(3.0f, 2.0f) + Math.Pow(5.0f, 2.0f) + Math.Pow(6.0f, 2.0f);

            Assert.AreEqual((float)len, a.LengthSquared);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Length()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var len = Math.Pow(2.0f, 2.0f) + Math.Pow(3.0f, 2.0f) + Math.Pow(5.0f, 2.0f) + Math.Pow(6.0f, 2.0f);
            var lenSquared = Math.Sqrt(len);

            Assert.AreEqual((float)lenSquared, a.Length);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void IsZero()
        {
            var a = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            var b = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var c = new Vector4(2.0f, 0.0f, 0.0f, 0.0f);
            var d = new Vector4(0.0f, 3.0f, 0.0f, 0.0f);
            var e = new Vector4(0.0f, 0.0f, 5.0f, 0.0f);
            var f = new Vector4(0.0f, 0.0f, 0.0f, 6.0f);

            Assert.IsTrue(a.IsZero);
            Assert.IsFalse(b.IsZero);
            Assert.IsFalse(c.IsZero);
            Assert.IsFalse(d.IsZero);
            Assert.IsFalse(e.IsZero);
            Assert.IsFalse(f.IsZero);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Negated()
        {
            var a = new Vector4(1.0f, -2.0f, -3.0f, 4.0f);
            var expected = new Vector4(-1.0f, 2.0f, 3.0f, -4.0f);

            Assert.AreEqual(expected, a.Negated());
            Assert.AreEqual(Vector4.Zero, Vector4.Zero.Negated());
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Normalize()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var expected = new Vector4(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length,
                a.W / a.Length);

            var result = a;
            result.Normalize();
            
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Normalized()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var expected = new Vector4(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length,
                a.W / a.Length);

            Assert.AreEqual(expected, a.Normalized());
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void NormalizeStatic()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var expected = new Vector4(
                a.X / a.Length,
                a.Y / a.Length,
                a.Z / a.Length,
                a.W / a.Length);

            Assert.AreEqual(expected, Vector4.Normalize(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizeVectorOfZeroLength()
        {
            var a = Vector4.Zero;
            a.Normalize();
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotNormalizedVectorOfZeroLength()
        {
            var a = Vector4.Zero;
            a.Normalized();
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void DefaultToString()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            var s = "<x: 2.5, y: 3, z: 5, w: 6.2>";

            Assert.AreEqual(s, a.ToString());
            Assert.AreEqual(s, a.ToString(null));
            Assert.AreEqual(s, a.ToString(null, null));
            Assert.AreEqual(s, a.ToString(string.Empty));
            Assert.AreEqual(s, a.ToString(string.Empty, null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void ToStringFullFormat()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            var s = "<Vector4 x: 2.5, y: 3, z: 5, w: 6.2>";

            Assert.AreEqual(s, a.ToString("F"));
            Assert.AreEqual(s, a.ToString("F", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void ToStringJsonFormat()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            var s = "{ \"x\": \"2.5\", \"y\": \"3\", \"z\": \"5\", \"w\": \"6.2\" }";

            Assert.AreEqual(s, a.ToString("J"));
            Assert.AreEqual(s, a.ToString("J", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void ToStringValueFormat()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            var s = "2.5, 3, 5, 6.2";

            Assert.AreEqual(s, a.ToString("V"));
            Assert.AreEqual(s, a.ToString("V", null));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException1()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            a.ToString("LOL");
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        [ExpectedException(typeof(FormatException))]
        public void ToStringUnsupportedFormatThrowsException2()
        {
            var a = new Vector4(2.5f, 3.0f, 5.0f, 6.2f);
            a.ToString("LOL", null);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Equals()
        {
            var a = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var b = new Vector4(2.0f, 3.0f, 5.0f, 6.0f);
            var c = new Vector4(2.0f, 0.0f, 0.0f, 0.0f);
            var d = new Vector4(0.0f, 3.0f, 0.0f, 0.0f);
            var e = new Vector4(0.0f, 0.0f, 5.0f, 0.0f);
            var f = new Vector4(0.0f, 0.0f, 0.0f, 6.0f);
            var g = new Vector4(2.0f, 0.0f, 0.0f, 0.0f);
            var h = new Vector4(2.0f, 3.0f, 0.0f, 0.0f);
            var i = new Vector4(2.0f, 3.0f, 5.0f, 0.0f);
            var j = new Vector4(0.0f, 3.0f, 5.0f, 6.0f);

            Assert.IsFalse(a.Equals(null));

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals((object) a));

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);

            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(b.Equals((object)a));
            Assert.IsTrue(b == a);
            Assert.IsFalse(b != a);

            Assert.IsFalse(a.Equals(c));
            Assert.IsFalse(a.Equals((object)c));
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);

            Assert.IsFalse(a.Equals(d));
            Assert.IsFalse(a.Equals((object)d));
            Assert.IsFalse(a == d);
            Assert.IsTrue(a != d);

            Assert.IsFalse(a.Equals(e));
            Assert.IsFalse(a.Equals((object)e));
            Assert.IsFalse(a == e);
            Assert.IsTrue(a != e);

            Assert.IsFalse(a.Equals(f));
            Assert.IsFalse(a.Equals((object)f));
            Assert.IsFalse(a == f);
            Assert.IsTrue(a != f);

            Assert.IsFalse(a.Equals(g));
            Assert.IsFalse(a.Equals((object)g));
            Assert.IsFalse(a == g);
            Assert.IsTrue(a != g);

            Assert.IsFalse(a.Equals(h));
            Assert.IsFalse(a.Equals((object)h));
            Assert.IsFalse(a == h);
            Assert.IsTrue(a != h);

            Assert.IsFalse(a.Equals(i));
            Assert.IsFalse(a.Equals((object)i));
            Assert.IsFalse(a == i);
            Assert.IsTrue(a != i);

            Assert.IsFalse(a.Equals(j));
            Assert.IsFalse(a.Equals((object)j));
            Assert.IsFalse(a == j);
            Assert.IsTrue(a != j);
        }
       
        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void TestGetHashCode()
        {
            // Simple unit test to check that we're getting some kind of result.
            var a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            var b = new Vector4(a);
            var c = new Vector4(3.0f, 1.0f, 4.0f, 3.0f);

            Assert.AreEqual(a.GetHashCode(), a.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Negation()
        {
            var a = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);
            var expected = new Vector4(-1.0f, 2.0f, -3.0f, -4.0f);

            Assert.AreEqual(expected, -a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void NegateStatic()
        {
            var a = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);
            var expected = new Vector4(-1.0f, 2.0f, -3.0f, -4.0f);

            Assert.AreEqual(expected, Vector4.Negate(a));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Positation()
        {
            var a = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);
            var expected = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);

            Assert.AreEqual(expected, +a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Addition()
        {
            var a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            var b = new Vector4(2.5f, -1.0f, 4.0f, -0.5f);
            var expected = new Vector4(3.5f, 1.0f, 7.0f, 3.5f);

            Assert.AreEqual(expected, Vector4.Add(a, b));
            Assert.AreEqual(expected, a + b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Subtraction()
        {
            var a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            var b = new Vector4(2.5f, -1.0f, 4.0f, -0.5f);
            var expected = new Vector4(-1.5f, 3.0f, -1.0f, 4.5f);

            Assert.AreEqual(expected, Vector4.Subtract(a, b));
            Assert.AreEqual(expected, a - b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Multiplication()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = -3.0f;
            var expected = new Vector4(-3.0f, -6.0f, 9.0f, -12.0f);

            Assert.AreEqual(expected, Vector4.Multiply(a, b));
            Assert.AreEqual(expected, a * b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Division()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = -2.0f;
            var expected = new Vector4(-0.5f, -1.0f, 1.5f, -2.0f);

            Assert.AreEqual(expected, Vector4.Divide(a, b));
            Assert.AreEqual(expected, a / b);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Cross()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            Vector4.Cross(a, a);
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Distance()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = new Vector4(2.0f, -1.0f, 0.5f, 0.0f);
            var expected = (float)Math.Sqrt(
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f) +
                (0.5f - -3.0f) * (0.5f - -3.0f) +
                (0.0f - 4.0f) * (0.0f - 4.0f));

            Assert.AreEqual(expected, Vector4.Distance(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void DistanceSquared()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = new Vector4(2.0f, -1.0f, 0.5f, 0.0f);
            var expected =
                (2.0f - 1.0f) * (2.0f - 1.0f) +
                (-1.0f - 2.0f) * (-1.0f - 2.0f) +
                (0.5f - -3.0f) * (0.5f - -3.0f) +
                (0.0f - 4.0f) * (0.0f - 4.0f);

            Assert.AreEqual(expected, Vector4.DistanceSquared(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void DotProduct()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = new Vector4(2.0f, -1.0f, 0.5f, 0.0f);
            var expected = 1.0f * 2.0f + 2.0f * -1.0f + -3.0f * 0.5f + 4.0f * 0.0f;

            Assert.AreEqual(expected, Vector4.Dot(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Max()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = new Vector4(2.0f, -1.0f, 0.5f, 0.0f);
            var expected = new Vector4(2.0f, 2.0f, 0.5f, 4.0f);

            Assert.AreEqual(expected, Vector4.Max(a, b));
        }

        [TestMethod]
        [TestCategory("Forge/Vector4")]
        public void Min()
        {
            var a = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
            var b = new Vector4(2.0f, -1.0f, 0.5f, 0.0f);
            var expected = new Vector4(1.0f, -1.0f, -3.0f, 0.0f);

            Assert.AreEqual(expected, Vector4.Min(a, b));
        }
    }
}
