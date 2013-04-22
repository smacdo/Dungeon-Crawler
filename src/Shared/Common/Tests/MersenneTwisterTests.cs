#pragma warning disable 1591            // Disable all XML Comment warnings in this file

using NUnit.Framework;
using System;

namespace Scott.Common.Tests
{
    [TestFixture]
    internal class MersenneTwisterTests
    {
        [Test]
        public void GenerateRandomUInt()
        {
            MersenneTwister m = new MersenneTwister();
            m.NextUInt();
        }
    }
}
