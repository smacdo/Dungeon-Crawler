using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MathHelperTests
    {
        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NormalizeToZeroOneRange_Float()
        {
            // 0/4
            Assert.AreEqual( 0.00f, MathHelper.NormalizeToZeroOneRange( 0.0f, 0.0f, 4.0f ) );

            // 1/4
            Assert.AreEqual( 0.25f, MathHelper.NormalizeToZeroOneRange( 1.0f, 0.0f, 4.0f ) );

            // 2/4
            Assert.AreEqual( 0.50f, MathHelper.NormalizeToZeroOneRange( 2.0f, 0.0f, 4.0f ) );

            // 3/4
            Assert.AreEqual( 0.75f, MathHelper.NormalizeToZeroOneRange( 3.0f, 0.0f, 4.0f ) );

            // 4/4
            Assert.AreEqual( 1.00f, MathHelper.NormalizeToZeroOneRange( 4.0f, 0.0f, 4.0f ) );

            // 5/4
            Assert.AreEqual( 1.25f, MathHelper.NormalizeToZeroOneRange( 5.0f, 0.0f, 4.0f ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NormalizeToZeroOneRange_Double()
        {
            // 0/4
            Assert.AreEqual( 0.00, MathHelper.NormalizeToZeroOneRange( 0.0, 0.0, 4.0 ) );

            // 1/4
            Assert.AreEqual( 0.25, MathHelper.NormalizeToZeroOneRange( 1.0, 0.0, 4.0 ) );

            // 2/4
            Assert.AreEqual( 0.50, MathHelper.NormalizeToZeroOneRange( 2.0, 0.0, 4.0 ) );

            // 3/4
            Assert.AreEqual( 0.75, MathHelper.NormalizeToZeroOneRange( 3.0, 0.0, 4.0 ) );

            // 4/4
            Assert.AreEqual( 1.00, MathHelper.NormalizeToZeroOneRange( 4.0, 0.0, 4.0 ) );

            // 5/4
            Assert.AreEqual( 1.25, MathHelper.NormalizeToZeroOneRange( 5.0, 0.0, 4.0 ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NormalizeToNegativeOneOneRange_Float()
        {
            // 0/4
            Assert.AreEqual(-1.0f, MathHelper.NormalizeToNegativeOneOneRange(0.0f, 0.0f, 4.0f));

            // 1/4
            Assert.AreEqual(-0.5f, MathHelper.NormalizeToNegativeOneOneRange(1.0f, 0.0f, 4.0f));

            // 2/4
            Assert.AreEqual(0.00f, MathHelper.NormalizeToNegativeOneOneRange(2.0f, 0.0f, 4.0f));

            // 3/4
            Assert.AreEqual(0.50f, MathHelper.NormalizeToNegativeOneOneRange(3.0f, 0.0f, 4.0f));

            // 4/4
            Assert.AreEqual(1.00f, MathHelper.NormalizeToNegativeOneOneRange(4.0f, 0.0f, 4.0f));

            // 5/4
            Assert.AreEqual(1.50f, MathHelper.NormalizeToNegativeOneOneRange(5.0f, 0.0f, 4.0f));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NormalizeToNegativeOneOneRange_Double()
        {
            // 0/4
            Assert.AreEqual(-1.0, MathHelper.NormalizeToNegativeOneOneRange(0.0, 0.0, 4.0));

            // 1/4
            Assert.AreEqual(-0.5, MathHelper.NormalizeToNegativeOneOneRange(1.0, 0.0, 4.0));

            // 2/4
            Assert.AreEqual(0.00, MathHelper.NormalizeToNegativeOneOneRange(2.0, 0.0, 4.0));

            // 3/4
            Assert.AreEqual(0.50, MathHelper.NormalizeToNegativeOneOneRange(3.0, 0.0, 4.0));

            // 4/4
            Assert.AreEqual(1.00, MathHelper.NormalizeToNegativeOneOneRange(4.0, 0.0, 4.0));

            // 5/4
            Assert.AreEqual(1.50, MathHelper.NormalizeToNegativeOneOneRange(5.0, 0.0, 4.0));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void DegreesToRadians()
        {
            Assert.AreEqual( 0.0f, MathHelper.DegreeToRadian( 0.0f ) );
            Assert.AreEqual( 0.785398163f, MathHelper.DegreeToRadian( 45.0f ) );
            Assert.AreEqual( 1.57079633f, MathHelper.DegreeToRadian( 90.0f ) );
            Assert.AreEqual( 3.14159265f, MathHelper.DegreeToRadian( 180.0f ) );
            Assert.AreEqual( 6.28318531f, MathHelper.DegreeToRadian( 360.0f ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void RadiansToDegrees()
        {
            Assert.AreEqual( 0.0f, MathHelper.RadianToDegree( 0.0f ) );
            Assert.AreEqual( 45.0f, MathHelper.RadianToDegree( 0.785398163f ) );
            Assert.AreEqual( 90.0f, MathHelper.RadianToDegree( 1.57079633f ) );
            Assert.AreEqual( 180.0f, MathHelper.RadianToDegree( 3.14159265f ) );
            Assert.AreEqual( 360.0f, MathHelper.RadianToDegree( 6.28318531f ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void SmallestAngleBetweenRadians()
        {
            float zero = MathHelper.DegreeToRadian( 0.0f );
            float p10 = MathHelper.DegreeToRadian( 10.0f );
            float p40 = MathHelper.DegreeToRadian( 40.0f );
            float p50 = MathHelper.DegreeToRadian( 50.0f );
            float p90 = MathHelper.DegreeToRadian( 90.0f );
            float p350 = MathHelper.DegreeToRadian( 350.0f );
            float p360 = MathHelper.DegreeToRadian( 360.0f );

            Assert.AreEqual( zero, MathHelper.SmallestAngleBetween( 0.0f, 0.0f ) );
            Assert.AreEqual( zero, MathHelper.SmallestAngleBetween( zero, p360 ), 0.00001f );
            Assert.AreEqual( p10, MathHelper.SmallestAngleBetween( p40, p50 ), 0.00001f );
            Assert.AreEqual( p40, MathHelper.SmallestAngleBetween( p50, p90 ), 0.00001f );
            Assert.AreEqual( p10, MathHelper.SmallestAngleBetween( p350, p360 ), 0.00001f );
            Assert.AreEqual( p50, MathHelper.SmallestAngleBetween( p350, p40 ), 0.00001f );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void SmallestAngleBetweenDegrees()
        {
            Assert.AreEqual( 0.0f, MathHelper.SmallestDegreeAngleBetween( 0.0f, 0.0f ) );
            Assert.AreEqual( 0.0f, MathHelper.SmallestDegreeAngleBetween( 0.0f, 360.0f ), 0.00001f );
            Assert.AreEqual( 10.0f, MathHelper.SmallestDegreeAngleBetween( 40.0f, 50.0f ), 0.00001f );
            Assert.AreEqual( 40.0f, MathHelper.SmallestDegreeAngleBetween( 50.0f, 90.0f ), 0.00001f );
            Assert.AreEqual( 10.0f, MathHelper.SmallestDegreeAngleBetween( 350.0f, 360.0f ), 0.00001f );
            Assert.AreEqual( 50.0f, MathHelper.SmallestDegreeAngleBetween( 350.0f, 40.0f ), 0.00001f );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NearlyEqualDouble()
        {
            Assert.IsTrue( MathHelper.NearlyEqual( 0.0, 0.0, 0.001 ) );
            Assert.IsTrue( MathHelper.NearlyEqual( 1.0, 1.0, 0.001 ) );
            Assert.IsTrue( MathHelper.NearlyEqual( 1.0001, 1.0001, 0.001 ) );
            Assert.IsFalse( MathHelper.NearlyEqual( 1.0001, 1.010, 0.001 ) );
            Assert.IsFalse(MathHelper.NearlyEqual(0.0, 1.0, 0.001));
            Assert.IsFalse(MathHelper.NearlyEqual(1.0, 0.0, 0.001));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NearlyEqualFloat()
        {
            Assert.IsTrue( MathHelper.NearlyEqual( 0.0f, 0.0f, 0.001f ) );
            Assert.IsTrue( MathHelper.NearlyEqual( 1.0f, 1.0f, 0.001f ) );
            Assert.IsTrue( MathHelper.NearlyEqual( 1.0001f, 1.0001f, 0.001f ) );
            Assert.IsFalse( MathHelper.NearlyEqual( 1.0001f, 1.010f, 0.001f ) );
            Assert.IsFalse(MathHelper.NearlyEqual(0.0f, 1.0f, 0.001f));
            Assert.IsFalse(MathHelper.NearlyEqual(1.0f, 0.0f, 0.001f));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void FastSin()
        {
            const float pi = (float) Math.PI;

            Assert.AreEqual( 0.0f, MathHelper.FastSin( 0.0f ) );
            Assert.AreEqual( 0.707106f, MathHelper.FastSin( pi / 4.0f ), 0.001f );
            Assert.AreEqual( 1.0f, MathHelper.FastSin( pi / 2.0f ), 0.001f );
            Assert.AreEqual( 0.0f, MathHelper.FastSin( pi ), 0.001f );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NextPowerOfTwoInt()
        {
            Assert.AreEqual( 1, MathHelper.NextPowerOf2( 0 ) );
            Assert.AreEqual( 1, MathHelper.NextPowerOf2( 1 ) );
            Assert.AreEqual( 2, MathHelper.NextPowerOf2( 2 ) );
            Assert.AreEqual( 4, MathHelper.NextPowerOf2( 3 ) );
            Assert.AreEqual( 4, MathHelper.NextPowerOf2( 4 ) );
            Assert.AreEqual( 8, MathHelper.NextPowerOf2( 5 ) );
            Assert.AreEqual( 65536, MathHelper.NextPowerOf2( 32769 ) );
            Assert.AreEqual( -2147483648, MathHelper.NextPowerOf2( 2147483646 ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NextPowerOfTwoUInt()
        {
            Assert.AreEqual( 1u, MathHelper.NextPowerOf2( 0u ) );
            Assert.AreEqual( 1u, MathHelper.NextPowerOf2( 1u ) );
            Assert.AreEqual( 2u, MathHelper.NextPowerOf2( 2u ) );
            Assert.AreEqual( 4u, MathHelper.NextPowerOf2( 3u ) );
            Assert.AreEqual( 4u, MathHelper.NextPowerOf2( 4u ) );
            Assert.AreEqual( 8u, MathHelper.NextPowerOf2( 5u ) );
            Assert.AreEqual( 65536u, MathHelper.NextPowerOf2( 32769u ) );
            Assert.AreEqual( 2147483648u, MathHelper.NextPowerOf2( 2147483646u ) );
            Assert.AreEqual( 0u, MathHelper.NextPowerOf2( 2147483649U ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NextPowerOfTwoLong()
        {
            Assert.AreEqual( 1, MathHelper.NextPowerOf2( 0L ) );
            Assert.AreEqual( 1, MathHelper.NextPowerOf2( 1L ) );
            Assert.AreEqual( 2, MathHelper.NextPowerOf2( 2L ) );
            Assert.AreEqual( 4, MathHelper.NextPowerOf2( 3L ) );
            Assert.AreEqual( 4, MathHelper.NextPowerOf2( 4L ) );
            Assert.AreEqual( 8, MathHelper.NextPowerOf2( 5L ) );
            Assert.AreEqual( 65536, MathHelper.NextPowerOf2( 32769L ) );
            Assert.AreEqual( 2147483648, MathHelper.NextPowerOf2( 2147483646L ) );
            Assert.AreEqual( -9223372036854775808, MathHelper.NextPowerOf2( 9223372036854775806L ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void NextPowerOfTwoULong()
        {
            Assert.AreEqual( 1u, MathHelper.NextPowerOf2( 0UL ) );
            Assert.AreEqual( 1u, MathHelper.NextPowerOf2( 1UL ) );
            Assert.AreEqual( 2u, MathHelper.NextPowerOf2( 2UL ) );
            Assert.AreEqual( 4u, MathHelper.NextPowerOf2( 3UL ) );
            Assert.AreEqual( 4u, MathHelper.NextPowerOf2( 4UL ) );
            Assert.AreEqual( 8u, MathHelper.NextPowerOf2( 5UL ) );
            Assert.AreEqual( 65536u, MathHelper.NextPowerOf2( 32769UL ) );
            Assert.AreEqual( 2147483648u, MathHelper.NextPowerOf2( 2147483646UL ) );
            Assert.AreEqual( 9223372036854775808u, MathHelper.NextPowerOf2( 9223372036854775806UL ) );
            Assert.AreEqual( 0u, MathHelper.NextPowerOf2( 18446744073709551615UL ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void LinearRemapFloat()
        {
            // 0/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(5.0f, MathHelper.LinearRemap(2.0f, 2.0f, 4.0f, 5.0f, 10.0f));

            // 2/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(7.5f, MathHelper.LinearRemap(3.0f, 2.0f, 4.0f, 5.0f, 10.0f));

            // 4/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(10.0f, MathHelper.LinearRemap(4.0f, 2.0f, 4.0f, 5.0f, 10.0f));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void LinearRemapDouble()
        {
            // 0/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(5.0, MathHelper.LinearRemap(2.0, 2.0, 4.0, 5.0, 10.0));

            // 2/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(7.5, MathHelper.LinearRemap(3.0, 2.0, 4.0, 5.0, 10.0));

            // 4/4: [2 - 4] ==> [5, 10]
            Assert.AreEqual(10.0, MathHelper.LinearRemap(4.0, 2.0, 4.0, 5.0, 10.0));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void Clamp()
        {
            Assert.AreEqual( 2.0f, MathHelper.Clamp( 1.5f, 2.0f, 3.0f ) );
            Assert.AreEqual( 2.0f, MathHelper.Clamp( 2.0f, 2.0f, 3.0f ) );
            Assert.AreEqual( 2.5f, MathHelper.Clamp( 2.5f, 2.0f, 3.0f ) );
            Assert.AreEqual( 3.0f, MathHelper.Clamp( 3.0f, 2.0f, 3.0f ) );
            Assert.AreEqual( 3.0f, MathHelper.Clamp( 4.0f, 2.0f, 3.0f ) );
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void Wrap()
        {
            Assert.AreEqual(0.0f, MathHelper.Wrap(0.0f, 0.0f, 2.0f));
            Assert.AreEqual(0.5f, MathHelper.Wrap(0.5f, 0.0f, 2.0f));
            Assert.AreEqual(1.0f, MathHelper.Wrap(1.0f, 0.0f, 2.0f));
            Assert.AreEqual(1.5f, MathHelper.Wrap(1.5f, 0.0f, 2.0f));
            Assert.AreEqual(0.0f, MathHelper.Wrap(2.0f, 0.0f, 2.0f));
            Assert.AreEqual(1.0f, MathHelper.Wrap(3.0f, 0.0f, 2.0f));
            Assert.AreEqual(1.0f, MathHelper.Wrap(5.0f, 0.0f, 2.0f));
            Assert.AreEqual(1.0f, MathHelper.Wrap(-1.0f, 0.0f, 2.0f));

            Assert.AreEqual(2.0f, MathHelper.Wrap(2.0f, 2.0f, 4.0f));
            Assert.AreEqual(2.5f, MathHelper.Wrap(2.5f, 2.0f, 4.0f));
            Assert.AreEqual(3.0f, MathHelper.Wrap(3.0f, 2.0f, 4.0f));
            Assert.AreEqual(3.5f, MathHelper.Wrap(3.5f, 2.0f, 4.0f));
            Assert.AreEqual(2.0f, MathHelper.Wrap(4.0f, 2.0f, 4.0f));
            Assert.AreEqual(3.0f, MathHelper.Wrap(5.0f, 2.0f, 4.0f));
            Assert.AreEqual(3.0f, MathHelper.Wrap(7.0f, 2.0f, 4.0f));
            Assert.AreEqual(3.0f, MathHelper.Wrap(-1.0f, 2.0f, 4.0f));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void GetDirectionTo()
        {
            // Test that zero or identical vector gives a zero direction vector.
            var one = new Vector2(1.0f, 1.0f);

            Assert.AreEqual(Vector2.Zero, MathHelper.GetDirectionTo(Vector2.Zero, Vector2.Zero));
            Assert.AreEqual(Vector2.Zero, MathHelper.GetDirectionTo(one, one));

            // Mirrored vectors.
            var a = new Vector2(2.0f, 3.0f);
            var b = new Vector2(-2.0f, -3.0f);

            var expected = Vector2.Normalize(new Vector2(-4.0f, -6.0f));
            Assert.AreEqual(expected, MathHelper.GetDirectionTo(a, b));

            expected = Vector2.Normalize(new Vector2(4.0f, 6.0f));
            Assert.AreEqual(expected, MathHelper.GetDirectionTo(b, a));

            // Two aribtrary vectors.
            a = new Vector2(-3.0f, 2.0f);
            b = new Vector2(6.0f, 1.0f);

            expected = Vector2.Normalize(new Vector2(9.0f, -1.0f));
            Assert.AreEqual(expected, MathHelper.GetDirectionTo(a, b));

            expected = Vector2.Normalize(new Vector2(-9.0f, 1.0f));
            Assert.AreEqual(expected, MathHelper.GetDirectionTo(b, a));
        }

        [TestMethod]
        [TestCategory("Forge/MathHelper")]
        public void RotateVector()
        {
            float p0 = MathHelper.DegreeToRadian(0.0f);
            float p45 = MathHelper.DegreeToRadian(45.0f);
            float p90 = MathHelper.DegreeToRadian(90.0f);
            float p180 = MathHelper.DegreeToRadian(180.0f);
            float p270 = MathHelper.DegreeToRadian(270.0f);

            // Test basic rotations from starting at (0, 1).
            Vector2 v = new Vector2(1.0f, 0.0f);

            AssertHelper.AreNearlyEqual(new Vector2(1.0f, 0.0f), MathHelper.Rotate(v, p0));
            AssertHelper.AreNearlyEqual(new Vector2(0.7071068f, 0.7071068f), MathHelper.Rotate(v, p45));
            AssertHelper.AreNearlyEqual(new Vector2(0.0f, 1.0f), MathHelper.Rotate(v, p90));
            AssertHelper.AreNearlyEqual(new Vector2(-1.0f, 0.0f), MathHelper.Rotate(v, p180));
            AssertHelper.AreNearlyEqual(new Vector2(0.0f, -1.0f), MathHelper.Rotate(v, p270));

            // Test rotation an arbitrary vector.
            //  TODO: Write unit test for this case.
        }
    }
}
