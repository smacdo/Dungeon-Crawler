#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;

namespace Scott.Common.Tests
{
    [TestFixture]
    public class InterpolationFuncsTests
    {
        [Test]
        public void LerpFloat()
        {
            Assert.AreEqual( 1.0f, InterpolationFuncs.Lerp( 2.0f, 4.0f, -0.5f ) );
            Assert.AreEqual( 2.0f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 0.00f ) );
            Assert.AreEqual( 2.5f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 0.25f ) );
            Assert.AreEqual( 3.0f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 0.50f ) );
            Assert.AreEqual( 3.5f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 0.75f ) );
            Assert.AreEqual( 4.0f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 1.00f ) );
            Assert.AreEqual( 6.0f, InterpolationFuncs.Lerp( 2.0f, 4.0f, 2.00f ) );
        }

        [Test]
        public void LerpDouble()
        {
            Assert.AreEqual( 1.0, InterpolationFuncs.Lerp( 2.0, 4.0, -0.5 ) );
            Assert.AreEqual( 2.0, InterpolationFuncs.Lerp( 2.0, 4.0, 0.00 ) );
            Assert.AreEqual( 2.5, InterpolationFuncs.Lerp( 2.0, 4.0, 0.25 ) );
            Assert.AreEqual( 3.0, InterpolationFuncs.Lerp( 2.0, 4.0, 0.50 ) );
            Assert.AreEqual( 3.5, InterpolationFuncs.Lerp( 2.0, 4.0, 0.75 ) );
            Assert.AreEqual( 4.0, InterpolationFuncs.Lerp( 2.0, 4.0, 1.00 ) );
            Assert.AreEqual( 6.0, InterpolationFuncs.Lerp( 2.0, 4.0, 2.00 ) );
        }

        [Test]
        public void UnlerpFloat()
        {
            Assert.AreEqual( -0.5f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 1.00f ) );
            Assert.AreEqual( 0.00f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 2.00f ) );
            Assert.AreEqual( 0.25f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 2.50f ) );
            Assert.AreEqual( 0.50f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 3.00f ) );
            Assert.AreEqual( 0.75f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 3.50f ) );
            Assert.AreEqual( 1.00f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 4.00f ) );
            Assert.AreEqual( 2.00f, InterpolationFuncs.Unlerp( 2.0f, 4.0f, 6.00f ) );
        }

        [Test]
        public void UnlerpDouble()
        {
            Assert.AreEqual( -0.5, InterpolationFuncs.Unlerp( 2.0, 4.0, 1.00 ) );
            Assert.AreEqual( 0.00, InterpolationFuncs.Unlerp( 2.0, 4.0, 2.00 ) );
            Assert.AreEqual( 0.25, InterpolationFuncs.Unlerp( 2.0, 4.0, 2.50 ) );
            Assert.AreEqual( 0.50, InterpolationFuncs.Unlerp( 2.0, 4.0, 3.00 ) );
            Assert.AreEqual( 0.75, InterpolationFuncs.Unlerp( 2.0, 4.0, 3.50 ) );
            Assert.AreEqual( 1.00, InterpolationFuncs.Unlerp( 2.0, 4.0, 4.00 ) );
            Assert.AreEqual( 2.00, InterpolationFuncs.Unlerp( 2.0, 4.0, 6.00 ) );
        }
    }
}
