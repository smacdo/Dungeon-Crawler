using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Geometry.Tests
{
    /// <summary>
    /// Tests for RectangleF
    /// </summary>
    [TestFixture]
    [Category("Common/Geometry")]
    internal class RectangleFTests
    {
        [Test]
        public void DefaultRect()
        {
            RectangleF rect = default(RectangleF);

            Assert.AreEqual( 0.0f, rect.X );
            Assert.AreEqual( 0.0f, rect.Y );
            Assert.AreEqual( 0.0f, rect.Width );
            Assert.AreEqual( 0.0f, rect.Height );
        }

        [Test]
        public void CreateWithPointAndDimensions()
        {
            RectangleF rect = new RectangleF( 2.5f, 3.0f, 4.5f, 6.2f );

            Assert.AreEqual( 2.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 4.5f, rect.Width );
            Assert.AreEqual( 6.2f, rect.Height );
        }

        [Test]
        public void IsRectEmpty()
        {
            // both width and height are empty
            RectangleF rectA = new RectangleF( 2.5f, -3.0f, 0.0f, 0.0f );
            Assert.IsTrue( rectA.IsEmpty );

            // only width is zero
            RectangleF rectB = new RectangleF( 2.5f, -3.0f, 0.0f, 1.0f );
            Assert.IsTrue( rectB.IsEmpty );

            // only height is zero
            RectangleF rectC = new RectangleF( 2.5f, -3.0f, 1.0f, 0.0f );
            Assert.IsTrue( rectC.IsEmpty );

            // neither width nor height is zero
            RectangleF rectD = new RectangleF( 2.5f, -3.0f, 1.0f, 1.0f );
            Assert.IsFalse( rectD.IsEmpty );
        }

        [Test]
        public void DefaultRectIsEmpty()
        {
            RectangleF rect = default( RectangleF );
            Assert.IsTrue( rect.IsEmpty );
        }

        [Test]
        public void EmptyRectFieldIsEmpty()
        {
            RectangleF rect = RectangleF.Empty;
            Assert.IsTrue( rect.IsEmpty );
        }

        [Test]
        public void X()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( 2.5f, rect.X );
        }

        [Test]
        public void Y()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( -3.0f, rect.Y );
        }

        [Test]
        public void SetX()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            rect.X = 7.0f;
            Assert.AreEqual( 7.0f, rect.X );
        }

        [Test]
        public void SetY()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            rect.Y = 9.0f;
            Assert.AreEqual( 9.0f, rect.Y );
        }

        [Test]
        public void Width()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( 1.2f, rect.Width );
        }

        [Test]
        public void Height()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( 0.5f, rect.Height );
        }

        [Test]
        public void SetWidth()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            rect.Width = 7.0f;
            Assert.AreEqual( 7.0f, rect.Width );
        }

        [Test]
        public void SetHeight()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            rect.Height = 9.0f;
            Assert.AreEqual( 9.0f, rect.Height );
        }

        [Test]
        public void Top()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( -3.0f, rect.Top );
        }

        [Test]
        public void Left()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( 2.5f, rect.Left );
        }

        [Test]
        public void Bottom()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( -2.5f, rect.Bottom );
        }

        [Test]
        public void Right()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual( 3.7f, rect.Right );
        }

        [Test]
        public void Location()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Vector2 location = rect.Location;

            Assert.AreEqual( 2.5f, location.X );
            Assert.AreEqual( -3.0f, location.Y );
        }

        [Test]
        public void Size()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Vector2 size = rect.Size;

            Assert.AreEqual( 1.2f, size.X );
            Assert.AreEqual( 0.5f, size.Y );
        }
    }
}
