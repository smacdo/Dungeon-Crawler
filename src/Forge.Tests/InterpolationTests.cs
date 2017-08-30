using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Tests
{
    [TestClass]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class InterpolationTests
    {
        [TestMethod]
        [TestCategory("Forge/Interpolation")]
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
        [TestCategory("Forge/Interpolation")]
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
        [TestCategory("Forge/Interpolation")]
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
        [TestCategory("Forge/Interpolation")]
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

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void SmoothStep()
        {
            // 0/4
            Assert.AreEqual(0.0f, Interpolation.SmoothStep(0.0f));
            Assert.AreEqual(2.0f, Interpolation.SmoothStep(2.0f, 4.0f, 0.0f));

            // 1/4
            float expected = 3.0f * (float) Math.Pow(0.25f, 2) - 2.0f * (float) Math.Pow(0.25, 3);
            Assert.AreEqual(expected, Interpolation.SmoothStep(0.25f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.SmoothStep(2.0f, 4.0f, 0.25f));

            // 2/4
            Assert.AreEqual(0.5f, Interpolation.SmoothStep(0.5f));
            Assert.AreEqual(3.0f, Interpolation.SmoothStep(2.0f, 4.0f, 0.5f));

            // 3/4
            expected = 3.0f * (float) Math.Pow(0.75f, 2) - 2.0f * (float) Math.Pow(0.75, 3);
            Assert.AreEqual(expected, Interpolation.SmoothStep(0.75f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.SmoothStep(2.0f, 4.0f, 0.75f));

            // 4/4
            Assert.AreEqual(1.0f, Interpolation.SmoothStep(1.0f));
            Assert.AreEqual(4.0f, Interpolation.SmoothStep(2.0f, 4.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void SmootherStep()
        {
            // 0/4
            Assert.AreEqual(0.0f, Interpolation.SmootherStep(0.0f));
            Assert.AreEqual(2.0f, Interpolation.SmootherStep(2.0f, 4.0f, 0.0f));

            // 1/4
            float expected =
                6.0f * (float) Math.Pow(0.25f, 5) -
                15.0f * (float) Math.Pow(0.25, 4) +
                10.0f * (float) Math.Pow(0.25, 3);
            Assert.AreEqual(expected, Interpolation.SmootherStep(0.25f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.SmootherStep(2.0f, 4.0f, 0.25f));

            // 2/4
            Assert.AreEqual(0.5f, Interpolation.SmootherStep(0.5f));
            Assert.AreEqual(3.0f, Interpolation.SmootherStep(2.0f, 4.0f, 0.5f));

            // 3/4
            expected =
                6.0f * (float) Math.Pow(0.75f, 5) -
                15.0f * (float) Math.Pow(0.75, 4) +
                10.0f * (float) Math.Pow(0.75, 3);
            Assert.AreEqual(expected, Interpolation.SmootherStep(0.75f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.SmootherStep(2.0f, 4.0f, 0.75f));

            // 4/4
            Assert.AreEqual(1.0f, Interpolation.SmootherStep(1.0f));
            Assert.AreEqual(4.0f, Interpolation.SmootherStep(2.0f, 4.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void Sinerp()
        {
            // 0/4
            Assert.AreEqual(0.0f, Interpolation.Sinerp(0.0f));
            Assert.AreEqual(2.0f, Interpolation.Sinerp(2.0f, 4.0f, 0.0f));

            // 1/4
            float expected = (float) Math.Sin(0.25f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Sinerp(0.25f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Sinerp(2.0f, 4.0f, 0.25f));

            // 2/4
            expected = (float) Math.Sin(0.50f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Sinerp(0.5f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Sinerp(2.0f, 4.0f, 0.5f));

            // 3/4
            expected = (float) Math.Sin(0.75f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Sinerp(0.75f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Sinerp(2.0f, 4.0f, 0.75f));

            // 4/4
            Assert.AreEqual(1.0f, Interpolation.Sinerp(1.0f));
            Assert.AreEqual(4.0f, Interpolation.Sinerp(2.0f, 4.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void Coserp()
        {
            // 0/4
            Assert.AreEqual(0.0f, Interpolation.Coserp(0.0f));
            Assert.AreEqual(2.0f, Interpolation.Coserp(2.0f, 4.0f, 0.0f));

            // 1/4
            float expected = 1.0f - (float) Math.Cos(0.25f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Coserp(0.25f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Coserp(2.0f, 4.0f, 0.25f));

            // 2/4
            expected = 1.0f - (float) Math.Cos(0.50f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Coserp(0.5f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Coserp(2.0f, 4.0f, 0.5f));

            // 3/4
            expected = 1.0f - (float) Math.Cos(0.75f * Math.PI * 0.5f);
            Assert.AreEqual(expected, Interpolation.Coserp(0.75f));
            Assert.AreEqual(2.0f + 2.0f * expected, Interpolation.Coserp(2.0f, 4.0f, 0.75f));

            // 4/4
            Assert.AreEqual(1.0f, Interpolation.Coserp(1.0f));
            Assert.AreEqual(4.0f, Interpolation.Coserp(2.0f, 4.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void Boing()
        {
            // 0/4
            Assert.AreEqual(0.0f, Interpolation.Boing(0.0f));
            Assert.AreEqual(2.0f, Interpolation.Boing(2.0f, 4.0f, 0.0f));

            // Test quarters to see if their values are clsoe to what we expect.
            float firstQuarter = Interpolation.Boing(0.25f);
            float secondQuarter = Interpolation.Boing(0.50f);
            float thirdQuarter = Interpolation.Boing(0.75f);

            Assert.IsTrue(firstQuarter < secondQuarter);
            Assert.IsTrue(thirdQuarter < secondQuarter);

            // 4/4
            Assert.AreEqual(1.0f, Interpolation.Boing(1.0f));
            Assert.AreEqual(4.0f, Interpolation.Boing(2.0f, 4.0f, 1.0f));
        }

        [TestMethod]
        [TestCategory("Forge/Interpolation")]
        public void Bounce()
        {
            // Not sure the best way to test bounce values... let's verify the start/end values are expected.
            // 0/4
            Assert.IsTrue(0.01f > Interpolation.Bounce(0.0f));
            Assert.IsTrue(2.01f > Interpolation.Bounce(2.0f, 4.0f, 0.0f));

            // 4/4
            Assert.IsTrue(0.01f > Interpolation.Bounce(0.0f));
            Assert.IsTrue(2.01f > Interpolation.Bounce(2.0f, 4.0f, 1.0f));
        }
    }
}
