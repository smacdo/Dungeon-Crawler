using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class InterpolationFuncsTests
    {
        [TestMethod]
        public void LerpFloat()
        {
            Assert.AreEqual( 1.0f, Interpolation.Lerp( 2.0f, 4.0f, -0.5f ) );
            Assert.AreEqual( 2.0f, Interpolation.Lerp( 2.0f, 4.0f, 0.00f ) );
            Assert.AreEqual( 2.5f, Interpolation.Lerp( 2.0f, 4.0f, 0.25f ) );
            Assert.AreEqual( 3.0f, Interpolation.Lerp( 2.0f, 4.0f, 0.50f ) );
            Assert.AreEqual( 3.5f, Interpolation.Lerp( 2.0f, 4.0f, 0.75f ) );
            Assert.AreEqual( 4.0f, Interpolation.Lerp( 2.0f, 4.0f, 1.00f ) );
            Assert.AreEqual( 6.0f, Interpolation.Lerp( 2.0f, 4.0f, 2.00f ) );
        }

        [TestMethod]
        public void LerpDouble()
        {
            Assert.AreEqual( 1.0, Interpolation.Lerp( 2.0, 4.0, -0.5 ) );
            Assert.AreEqual( 2.0, Interpolation.Lerp( 2.0, 4.0, 0.00 ) );
            Assert.AreEqual( 2.5, Interpolation.Lerp( 2.0, 4.0, 0.25 ) );
            Assert.AreEqual( 3.0, Interpolation.Lerp( 2.0, 4.0, 0.50 ) );
            Assert.AreEqual( 3.5, Interpolation.Lerp( 2.0, 4.0, 0.75 ) );
            Assert.AreEqual( 4.0, Interpolation.Lerp( 2.0, 4.0, 1.00 ) );
            Assert.AreEqual( 6.0, Interpolation.Lerp( 2.0, 4.0, 2.00 ) );
        }

        [TestMethod]
        public void UnlerpFloat()
        {
            Assert.AreEqual( -0.5f, Interpolation.Unlerp( 2.0f, 4.0f, 1.00f ) );
            Assert.AreEqual( 0.00f, Interpolation.Unlerp( 2.0f, 4.0f, 2.00f ) );
            Assert.AreEqual( 0.25f, Interpolation.Unlerp( 2.0f, 4.0f, 2.50f ) );
            Assert.AreEqual( 0.50f, Interpolation.Unlerp( 2.0f, 4.0f, 3.00f ) );
            Assert.AreEqual( 0.75f, Interpolation.Unlerp( 2.0f, 4.0f, 3.50f ) );
            Assert.AreEqual( 1.00f, Interpolation.Unlerp( 2.0f, 4.0f, 4.00f ) );
            Assert.AreEqual( 2.00f, Interpolation.Unlerp( 2.0f, 4.0f, 6.00f ) );
        }

        [TestMethod]
        public void UnlerpDouble()
        {
            Assert.AreEqual( -0.5, Interpolation.Unlerp( 2.0, 4.0, 1.00 ) );
            Assert.AreEqual( 0.00, Interpolation.Unlerp( 2.0, 4.0, 2.00 ) );
            Assert.AreEqual( 0.25, Interpolation.Unlerp( 2.0, 4.0, 2.50 ) );
            Assert.AreEqual( 0.50, Interpolation.Unlerp( 2.0, 4.0, 3.00 ) );
            Assert.AreEqual( 0.75, Interpolation.Unlerp( 2.0, 4.0, 3.50 ) );
            Assert.AreEqual( 1.00, Interpolation.Unlerp( 2.0, 4.0, 4.00 ) );
            Assert.AreEqual( 2.00, Interpolation.Unlerp( 2.0, 4.0, 6.00 ) );
        }
    }
}
