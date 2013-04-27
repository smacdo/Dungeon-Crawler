#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;

namespace Scott.Common.Tests
{
    [TestFixture]
    public class MathXTests
    {
        [Test]
        public void NormalizeToZeroOneRange_Float()
        {
            // 0/4
            Assert.AreEqual( 0.00f, MathX.NormalizeToZeroOneRange( 0.0f, 0.0f, 4.0f ) );

            // 1/4
            Assert.AreEqual( 0.25f, MathX.NormalizeToZeroOneRange( 1.0f, 0.0f, 4.0f ) );

            // 2/4
            Assert.AreEqual( 0.50f, MathX.NormalizeToZeroOneRange( 2.0f, 0.0f, 4.0f ) );

            // 3/4
            Assert.AreEqual( 0.75f, MathX.NormalizeToZeroOneRange( 3.0f, 0.0f, 4.0f ) );

            // 4/4
            Assert.AreEqual( 1.00f, MathX.NormalizeToZeroOneRange( 4.0f, 0.0f, 4.0f ) );

            // 5/4
            Assert.AreEqual( 1.25f, MathX.NormalizeToZeroOneRange( 5.0f, 0.0f, 4.0f ) );
        }

        [Test]
        public void NormalizeToZeroOneRange_Double()
        {
            // 0/4
            Assert.AreEqual( 0.00, MathX.NormalizeToZeroOneRange( 0.0, 0.0, 4.0 ) );

            // 1/4
            Assert.AreEqual( 0.25, MathX.NormalizeToZeroOneRange( 1.0, 0.0, 4.0 ) );

            // 2/4
            Assert.AreEqual( 0.50, MathX.NormalizeToZeroOneRange( 2.0, 0.0, 4.0 ) );

            // 3/4
            Assert.AreEqual( 0.75, MathX.NormalizeToZeroOneRange( 3.0, 0.0, 4.0 ) );

            // 4/4
            Assert.AreEqual( 1.00, MathX.NormalizeToZeroOneRange( 4.0, 0.0, 4.0 ) );

            // 5/4
            Assert.AreEqual( 1.25, MathX.NormalizeToZeroOneRange( 5.0, 0.0, 4.0 ) );
        }

        [Test]
        public void DegreesToRadians()
        {
            Assert.AreEqual( 0.0f, MathX.DegreeToRadian( 0.0f ) );
            Assert.AreEqual( 0.785398163f, MathX.DegreeToRadian( 45.0f ) );
            Assert.AreEqual( 1.57079633f, MathX.DegreeToRadian( 90.0f ) );
            Assert.AreEqual( 3.14159265f, MathX.DegreeToRadian( 180.0f ) );
            Assert.AreEqual( 6.28318531f, MathX.DegreeToRadian( 360.0f ) );
        }

        [Test]
        public void RadiansToDegrees()
        {
            Assert.AreEqual( 0.0f, MathX.RadianToDegree( 0.0f ) );
            Assert.AreEqual( 45.0f, MathX.RadianToDegree( 0.785398163f ) );
            Assert.AreEqual( 90.0f, MathX.RadianToDegree( 1.57079633f ) );
            Assert.AreEqual( 180.0f, MathX.RadianToDegree( 3.14159265f ) );
            Assert.AreEqual( 360.0f, MathX.RadianToDegree( 6.28318531f ) );
        }

        [Test]
        public void SmallestAngleBetweenRadians()
        {
            float zero = MathX.DegreeToRadian( 0.0f );
            float p10 = MathX.DegreeToRadian( 10.0f );
            float p40 = MathX.DegreeToRadian( 40.0f );
            float p50 = MathX.DegreeToRadian( 50.0f );
            float p90 = MathX.DegreeToRadian( 90.0f );
            float p350 = MathX.DegreeToRadian( 350.0f );
            float p360 = MathX.DegreeToRadian( 360.0f );

            Assert.AreEqual( zero, MathX.SmallestAngleBetween( 0.0f, 0.0f ) );
            Assert.AreEqual( zero, MathX.SmallestAngleBetween( zero, p360 ), 0.00001f );
            Assert.AreEqual( p10, MathX.SmallestAngleBetween( p40, p50 ), 0.00001f );
            Assert.AreEqual( p40, MathX.SmallestAngleBetween( p50, p90 ), 0.00001f );
            Assert.AreEqual( p10, MathX.SmallestAngleBetween( p350, p360 ), 0.00001f );
            Assert.AreEqual( p50, MathX.SmallestAngleBetween( p350, p40 ), 0.00001f );
        }

        [Test]
        public void SmallestAngleBetweenDegrees()
        {
            Assert.AreEqual( 0.0f, MathX.SmallestDegreeAngleBetween( 0.0f, 0.0f ) );
            Assert.AreEqual( 0.0f, MathX.SmallestDegreeAngleBetween( 0.0f, 360.0f ), 0.00001f );
            Assert.AreEqual( 10.0f, MathX.SmallestDegreeAngleBetween( 40.0f, 50.0f ), 0.00001f );
            Assert.AreEqual( 40.0f, MathX.SmallestDegreeAngleBetween( 50.0f, 90.0f ), 0.00001f );
            Assert.AreEqual( 10.0f, MathX.SmallestDegreeAngleBetween( 350.0f, 360.0f ), 0.00001f );
            Assert.AreEqual( 50.0f, MathX.SmallestDegreeAngleBetween( 350.0f, 40.0f ), 0.00001f );
        }

        [Test]
        public void NearlyEqualDouble()
        {
            Assert.IsTrue( MathX.NearlyEqual( 0.0, 0.0, 0.001 ) );
            Assert.IsTrue( MathX.NearlyEqual( 1.0, 1.0, 0.001 ) );
            Assert.IsTrue( MathX.NearlyEqual( 1.0001, 1.0001, 0.001 ) );
            Assert.IsFalse( MathX.NearlyEqual( 1.0001, 1.010, 0.001 ) );
        }

        [Test]
        public void NearlyEqualFloat()
        {
            Assert.IsTrue( MathX.NearlyEqual( 0.0f, 0.0f, 0.001f ) );
            Assert.IsTrue( MathX.NearlyEqual( 1.0f, 1.0f, 0.001f ) );
            Assert.IsTrue( MathX.NearlyEqual( 1.0001f, 1.0001f, 0.001f ) );
            Assert.IsFalse( MathX.NearlyEqual( 1.0001f, 1.010f, 0.001f ) );
        }

        [Test]
        public void FastSin()
        {
            const float pi = (float) Math.PI;

            Assert.AreEqual( 0.0f, MathX.FastSin( 0.0f ) );
            Assert.AreEqual( 0.707106f, MathX.FastSin( pi / 4.0f ), 0.001f );
            Assert.AreEqual( 1.0f, MathX.FastSin( pi / 2.0f ), 0.001f );
            Assert.AreEqual( 0.0f, MathX.FastSin( pi ), 0.001f );
        }

        [Test]
        public void NextPowerOfTwoInt()
        {
            Assert.AreEqual( 1, MathX.NextPowerOf2( 0 ) );
            Assert.AreEqual( 1, MathX.NextPowerOf2( 1 ) );
            Assert.AreEqual( 2, MathX.NextPowerOf2( 2 ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 3 ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 4 ) );
            Assert.AreEqual( 8, MathX.NextPowerOf2( 5 ) );
            Assert.AreEqual( 65536, MathX.NextPowerOf2( 32769 ) );
            Assert.AreEqual( -2147483648, MathX.NextPowerOf2( 2147483646 ) );
        }

        [Test]
        public void NextPowerOfTwoUInt()
        {
            Assert.AreEqual( 1, MathX.NextPowerOf2( 0u ) );
            Assert.AreEqual( 1, MathX.NextPowerOf2( 1u ) );
            Assert.AreEqual( 2, MathX.NextPowerOf2( 2u ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 3u ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 4u ) );
            Assert.AreEqual( 8, MathX.NextPowerOf2( 5u ) );
            Assert.AreEqual( 65536, MathX.NextPowerOf2( 32769u ) );
            Assert.AreEqual( 2147483648, MathX.NextPowerOf2( 2147483646u ) );
            Assert.AreEqual( 0, MathX.NextPowerOf2( 2147483649U ) );
        }

        [Test]
        public void NextPowerOfTwoLong()
        {
            Assert.AreEqual( 1, MathX.NextPowerOf2( 0L ) );
            Assert.AreEqual( 1, MathX.NextPowerOf2( 1L ) );
            Assert.AreEqual( 2, MathX.NextPowerOf2( 2L ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 3L ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 4L ) );
            Assert.AreEqual( 8, MathX.NextPowerOf2( 5L ) );
            Assert.AreEqual( 65536, MathX.NextPowerOf2( 32769L ) );
            Assert.AreEqual( 2147483648, MathX.NextPowerOf2( 2147483646L ) );
            Assert.AreEqual( -9223372036854775808, MathX.NextPowerOf2( 9223372036854775806L ) );
        }

        [Test]
        public void NextPowerOfTwoULong()
        {
            Assert.AreEqual( 1, MathX.NextPowerOf2( 0UL ) );
            Assert.AreEqual( 1, MathX.NextPowerOf2( 1UL ) );
            Assert.AreEqual( 2, MathX.NextPowerOf2( 2UL ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 3UL ) );
            Assert.AreEqual( 4, MathX.NextPowerOf2( 4UL ) );
            Assert.AreEqual( 8, MathX.NextPowerOf2( 5UL ) );
            Assert.AreEqual( 65536, MathX.NextPowerOf2( 32769UL ) );
            Assert.AreEqual( 2147483648, MathX.NextPowerOf2( 2147483646UL ) );
            Assert.AreEqual( 9223372036854775808, MathX.NextPowerOf2( 9223372036854775806UL ) );
            Assert.AreEqual( 0, MathX.NextPowerOf2( 18446744073709551615UL ) );
        }

        [Test]
        public void LinearRemapFloat()
        {
            Assert.AreEqual( 7.5f, MathX.LinearRemap( 3.0f, 2.0f, 4.0f, 5.0f, 10.0f ) );
        }

        [Test]
        public void Clamp()
        {
            Assert.AreEqual( 2.0f, MathX.Clamp( 1.5f, 2.0f, 3.0f ) );
            Assert.AreEqual( 2.0f, MathX.Clamp( 2.0f, 2.0f, 3.0f ) );
            Assert.AreEqual( 2.5f, MathX.Clamp( 2.5f, 2.0f, 3.0f ) );
            Assert.AreEqual( 3.0f, MathX.Clamp( 3.0f, 2.0f, 3.0f ) );
            Assert.AreEqual( 3.0f, MathX.Clamp( 4.0f, 2.0f, 3.0f ) );
        }

        [Test]
        public void RotateVector()
        {
            Vector2 start = new Vector2( 1.0f, 0.0f );

            float p45 = MathX.DegreeToRadian( 45.0f );
            float p90 = MathX.DegreeToRadian( 90.0f );
            float p180 = MathX.DegreeToRadian( 180.0f );
            float p270 = MathX.DegreeToRadian( 270.0f );

            // First from zero.
            AssertX.AreEqual( new Vector2( 0.7071068f, 0.7071068f ), MathX.Rotate( start, p45 ) );
            AssertX.AreEqual( new Vector2( 1.0f, 0.0f ), MathX.Rotate( start, 0.0f ) );
            AssertX.AreEqual( new Vector2( 0.0f, 1.0f ), MathX.Rotate( start, p90 ) );
            AssertX.AreEqual( new Vector2(-1.0f, 0.0f ), MathX.Rotate( start, p180 ) );
            AssertX.AreEqual( new Vector2( 0.0f,-1.0f ), MathX.Rotate( start, p270 ) );
        }
    }
}
