using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Geometry.Tests
{
    /// <summary>
    ///  Tests for BoundingAreaTests.
    /// </summary>
    [TestFixture]
    [Category( "Common/Geometry" )]
    internal class BoundingAreaTests
    {
        [Test]
        public void TestCreationPosAndSize()
        {
            BoundingArea area = new BoundingArea( new Vector2( 1.0f, 2.0f ),
                                                  new Vector2( 4.0f, 3.0f ) );

            Assert.AreEqual( new Vector2( 1.0f, 2.0f ), area.WorldPosition );
            Assert.AreEqual( new Vector2( 0.0f, 0.0f ), area.LocalOffset );
            Assert.AreEqual( new Vector2( 4.0f, 3.0f ), area.Size );
        }

        [Test]
        public void TestCreationPosSizeAndOffset()
        {
            BoundingArea area = new BoundingArea( new Vector2( 1.0f, 2.0f ),
                                                  new Vector2( 4.0f, 3.0f ),
                                                  new Vector2( 0.5f, 1.0f ) );

            Assert.AreEqual( new Vector2( 1.5f, 3.0f ), area.WorldPosition );
            Assert.AreEqual( new Vector2( 0.5f, 1.0f ), area.LocalOffset );
            Assert.AreEqual( new Vector2( 4.0f, 3.0f ), area.Size );
        }
    }
}
