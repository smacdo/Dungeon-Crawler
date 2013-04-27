using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Scott.Common.Tests
{
    internal static class AssertX
    {
        public static int FLOAT_THRESHOLD = 500;

        /// <summary>
        ///  Asserts that two vectors are equal within the accepted deviation.
        /// </summary>
        /// <param name="expected">Expected vector value.</param>
        /// <param name="actual">Actual vector value.</param>
        public static void AreEqual( Vector2 expected, Vector2 actual )
        {
            Assert.AreEqual( expected.X, actual.X, 0.0001f, "{0} != {1} (.X)".With( expected, actual ) );
            Assert.AreEqual( expected.Y, actual.Y, 0.0001f, "{0} != {1} (.Y)".With( expected, actual ) );
        }

        /// <summary>
        ///  Asserts that two vectors are equal within the accepted deviation.
        /// </summary>
        /// <param name="expected">Expected vector value.</param>
        /// <param name="actual">Actual vector value.</param>
        /// <param name="message">Message to print.</param>
        public static void AreEqual( Vector2 expected, Vector2 actual, string message )
        {
            Assert.AreEqual( expected.X, actual.X, 0.0001f, "{0}: {1} != {2} (.X)".With( message, expected, actual ) );
            Assert.AreEqual( expected.Y, actual.Y, 0.0001f, "{0}: {1} != {2} (.Y)".With( message, expected, actual ) );
        }
    }
}
