using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Scott.Forge;

namespace Scott.Forge.Tests
{
    /// <summary>
    ///  Additional unit test assertions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class AssertHelper
    {
        public static long MaxUnitDifference = 3;
        public static float MaxFloatDifference = 0.000001f;

        /// <summary>
        ///  Check if two vectors values are nearly equal.
        /// </summary>
        /// <param name="expected">Expected vector value.</param>
        /// <param name="actual">Actual vector value..</param>
        public static void AreNearlyEqual(Vector2 expected, Vector2 actual)
        {
            Assert.AreEqual(expected.X, actual.X, MaxFloatDifference, "{0} != {1} (v.X)".With(expected, actual));
            Assert.AreEqual(expected.Y, actual.Y, MaxFloatDifference, "{0} != {1} (v.Y)".With(expected, actual));
        }

        /// <summary>
        ///  Check if two double values are nearly equal.
        /// </summary>
        /// <remarks>
        ///  This was taken from:
        ///   https://msdn.microsoft.com/en-us/library/ya2zha7s%28v=vs.110%29.aspx
        /// </remarks>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <returns>If two double values are nearly equal.</returns>
        public static bool AreNearlyEqual(double a, double b)
        {
            long first = BitConverter.DoubleToInt64Bits(a);
            long second = BitConverter.DoubleToInt64Bits(b);

            // If the signs are different, return false except for +0 and -0.
            if ((first >> 63) != (second >> 63))
            {
                return (a == b);
            }

            long difference = Math.Abs(first - second);
            return (difference <= MaxUnitDifference);
        }

        /// <summary>
        ///  Check if two floating point values are nearly equal.
        /// </summary>
        /// <remarks>
        ///  This was taken from:
        ///   https://msdn.microsoft.com/en-us/library/ya2zha7s%28v=vs.110%29.aspx
        /// </remarks>
        /// <param name="a">First value to check.</param>
        /// <param name="b">Second value to check.</param>
        /// <returns>If two floating point values are nearly equal.</returns>
        public static bool AreNearlyEqual(float a, float b)
        {
            return AreNearlyEqual((double) a, (double) b);
        }
    }
}
