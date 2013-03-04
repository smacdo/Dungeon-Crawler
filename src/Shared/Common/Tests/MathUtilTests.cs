using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Common.Tests
{
    [TestFixture]
    public class MathUtilTests
    {
        [Test]
        public void NormalizeToZeroOneRange_Float()
        {
            // 0/4
            Assert.AreEqual( 0.00f, MathUtil.NormalizeToZeroOneRange( 0.0f, 0.0f, 4.0f ) );

            // 1/4
            Assert.AreEqual( 0.25f, MathUtil.NormalizeToZeroOneRange( 1.0f, 0.0f, 4.0f ) );

            // 2/4
            Assert.AreEqual( 0.50f, MathUtil.NormalizeToZeroOneRange( 2.0f, 0.0f, 4.0f ) );

            // 3/4
            Assert.AreEqual( 0.75f, MathUtil.NormalizeToZeroOneRange( 3.0f, 0.0f, 4.0f ) );

            // 4/4
            Assert.AreEqual( 1.00f, MathUtil.NormalizeToZeroOneRange( 4.0f, 0.0f, 4.0f ) );

            // 5/4
            Assert.AreEqual( 1.25f, MathUtil.NormalizeToZeroOneRange( 5.0f, 0.0f, 4.0f ) );
        }

        [Test]
        public void NormalizeToZeroOneRange_Double()
        {
            // 0/4
            Assert.AreEqual( 0.00, MathUtil.NormalizeToZeroOneRange( 0.0, 0.0, 4.0 ) );

            // 1/4
            Assert.AreEqual( 0.25, MathUtil.NormalizeToZeroOneRange( 1.0, 0.0, 4.0 ) );

            // 2/4
            Assert.AreEqual( 0.50, MathUtil.NormalizeToZeroOneRange( 2.0, 0.0, 4.0 ) );

            // 3/4
            Assert.AreEqual( 0.75, MathUtil.NormalizeToZeroOneRange( 3.0, 0.0, 4.0 ) );

            // 4/4
            Assert.AreEqual( 1.00, MathUtil.NormalizeToZeroOneRange( 4.0, 0.0, 4.0 ) );

            // 5/4
            Assert.AreEqual( 1.25, MathUtil.NormalizeToZeroOneRange( 5.0, 0.0, 4.0 ) );
        }
    }
}
