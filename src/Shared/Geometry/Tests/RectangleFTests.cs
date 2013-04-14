using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public void TestDefaultConstructor()
        {
            RectangleF rect = default(RectangleF);

            Assert.AreEqual( 0.0f, rect.X );
            Assert.AreEqual( 0.0f, rect.Y );
            Assert.AreEqual( 0.0f, rect.Width );
            Assert.AreEqual( 0.0f, rect.Height );
        }

        [Test]
        public void TestXYWidthHeightConstructor()
        {
            RectangleF rect = new RectangleF( 2.5f, 3.0f, 4.5f, 6.2f );

            Assert.AreEqual( 2.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 4.5f, rect.Width );
            Assert.AreEqual( 6.2f, rect.Height );
        }

        [Test]
        public void TestCopyConstructor()
        {
            RectangleF orig = new RectangleF( 2.5f, 3.0f, 4.5f, 6.2f );
            RectangleF rect = new RectangleF( orig );

            Assert.AreEqual( 2.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 4.5f, rect.Width );
            Assert.AreEqual( 6.2f, rect.Height );
        }

        [Test]
        public void TestXnaVectorsConstructor()
        {
            RectangleF rect = new RectangleF( new Vector2( 2.5f, 3.0f ), new Vector2( 4.5f, 6.2f ) );

            Assert.AreEqual( 2.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 4.5f, rect.Width );
            Assert.AreEqual( 6.2f, rect.Height );
        }

        [Test]
        public void TestXnaRectangleConstructor()
        {
            RectangleF rect = new RectangleF( new Rectangle( 2, 3, 4, 6 ) );

            Assert.AreEqual( 2.0f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 4.0f, rect.Width );
            Assert.AreEqual( 6.0f, rect.Height );
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
        public void TestTopLeft()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual(  2.5f, rect.TopLeft.X );
            Assert.AreEqual( -3.0f, rect.TopLeft.Y );
        }

        [Test]
        public void TestTopRight()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual(  3.7f, rect.TopRight.X );
            Assert.AreEqual( -3.0f, rect.TopRight.Y );
        }

        [Test]
        public void TestBottomLeft()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual(  2.5f, rect.BottomLeft.X );
            Assert.AreEqual( -2.5f, rect.BottomLeft.Y );
        }

        [Test]
        public void TestBottomRight()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Assert.AreEqual(  3.7f, rect.BottomRight.X );
            Assert.AreEqual( -2.5f, rect.BottomRight.Y );
        }

        [Test]
        public void TestLocation()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Vector2 location = rect.Location;

            Assert.AreEqual( 2.5f, location.X );
            Assert.AreEqual( -3.0f, location.Y );
        }

        [Test]
        public void TestSize()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );
            Vector2 size = rect.Size;

            Assert.AreEqual( 1.2f, size.X );
            Assert.AreEqual( 0.5f, size.Y );
        }

        [Test]
        public void TestCenter()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );

            Assert.AreEqual(  3.10f, rect.Center.X );
            Assert.AreEqual( -2.75f, rect.Center.Y );
        }

        [Test]
        public void TestSerialization()
        {
            RectangleF first = new RectangleF( 2.5f, -3.0f, 1.2f, 0.5f );

            // Serialize the object
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize( stream, first );

            // Now unserialize the object
            stream.Seek( 0, SeekOrigin.Begin );
            formatter = new BinaryFormatter();

            RectangleF second = (RectangleF) formatter.Deserialize( stream );

            // Test to make sure everything is the same
            Assert.AreEqual( second, first );
        }

        [Test]
        public void TestToRectangle()
        {
            RectangleF rect = new RectangleF( 2.5f, -3.0f, 1.2f, 1.51f );
            Rectangle xnaRect = rect.ToRectangle();

            // Test new values... they should have been rounded to the nearest int
            Assert.AreEqual(  2, xnaRect.X );
            Assert.AreEqual( -3, xnaRect.Y );
            Assert.AreEqual(  1, xnaRect.Width );
            Assert.AreEqual(  2, xnaRect.Height );
        }

        [Test]
        public void TestContainsXY()
        {
            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );

            // These values should be within the rectangle
            Assert.IsTrue( rect.Contains( rect.Center.X, rect.Center.Y ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( 1.50f, 2.50f ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( 1.75f, 3.50f ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( 2.00f, 4.95f ), "Should be inside rectangle" );

            // These values lie on the border and should be inside
            Assert.IsTrue( rect.Contains( rect.Left,  rect.Top ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Right, rect.Top ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Left,  rect.Bottom ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Right, rect.Bottom ), "On border and should be inside rectangle" );

            Assert.IsTrue( rect.Contains( rect.Center.X, rect.Top ),      "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Left,     rect.Center.Y ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Center.X, rect.Bottom ),   "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.Right,    rect.Center.Y ), "On border and should be inside rectangle" );

            // These values lie outside the rectangle and should not be contained
            Assert.IsFalse( rect.Contains( 0.0f, 0.0f ), "Should be outside the rectangle" );
            Assert.IsFalse( rect.Contains( 5.0f, 5.0f ), "Should be outside the rectangle" );

            Assert.IsFalse( rect.Contains( rect.Left, rect.Top - 0.01f ), "Should be outside the rectangle" );
            Assert.IsFalse( rect.Contains( rect.Left, rect.Bottom + 0.01f ), "Should be outside the rectangle" );
        }

        [Test]
        public void TestContainsVector2()
        {
            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );

            // These values should be within the rectangle
            Assert.IsTrue( rect.Contains( rect.Center ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( new Vector2( 1.50f, 2.50f ) ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( new Vector2( 1.75f, 3.50f ) ), "Should be inside rectangle" );
            Assert.IsTrue( rect.Contains( new Vector2( 2.00f, 4.95f ) ), "Should be inside rectangle" );

            // These values lie on the border and should be inside
            Assert.IsTrue( rect.Contains( rect.TopLeft ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.TopRight ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.BottomLeft ), "On border and should be inside rectangle" );
            Assert.IsTrue( rect.Contains( rect.BottomRight ), "On border and should be inside rectangle" );

            // These values lie outside the rectangle and should not be contained
            Assert.IsFalse( rect.Contains( new Vector2( 0.0f, 0.0f ) ), "Should be outside the rectangle" );
            Assert.IsFalse( rect.Contains( new Vector2( 5.0f, 5.0f ) ), "Should be outside the rectangle" );

            Assert.IsFalse( rect.Contains( new Vector2( rect.Left, rect.Top - 0.01f ) ), "Should be outside the rectangle" );
            Assert.IsFalse( rect.Contains( new Vector2( rect.Left, rect.Bottom + 0.01f ) ), "Should be outside the rectangle" );
        }

        [Test]
        public void TestContainsRectangle()
        {
            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            Assert.IsTrue( rect.Contains( rect ), "A rectangle should contain another rectangle of the same dimensions" );

            RectangleF smaller = new RectangleF( 1.2f, 2.4f, 1.0f, 1.0f );
            Assert.IsTrue( rect.Contains( smaller ), "A rectangle should contain another smaller rectangle" );

            RectangleF bigger = new RectangleF( 1.0f, 1.0f, 5.0f, 5.0f );
            Assert.IsFalse( rect.Contains( bigger ), "A rectangle should not contain a larger rectangle" );

            RectangleF intersected = new RectangleF( 0.8f, 2.2f, 1.5f, 3.0f );
            Assert.IsFalse( rect.Contains( intersected ), "A rectangle should not contain a rectangle that intersects it" );
        }

        [Test]
        public void TestInflate()
        {
            const float horizontal = 0.5f;
            const float vertical = 1.0f;

            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );

            rect.Inflate( horizontal, vertical );

            Assert.AreEqual( 0.5f, rect.X );
            Assert.AreEqual( 2.5f, rect.Width );
            Assert.AreEqual( 1.0f, rect.Y );
            Assert.AreEqual( 5.0f, rect.Height );
        }

        [Test]
        public void TestIntersection()
        {
            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            Assert.IsTrue( rect.Intersects( rect ), "A rectangle should intersect another rectangle of the same dimensions" );

            RectangleF smaller = new RectangleF( 1.2f, 2.1f, 1.0f, 1.0f );
            Assert.IsTrue( rect.Intersects( smaller ), "A rectangle should intersect a smaller rectangle inside it" );

            RectangleF bigger = new RectangleF( 1.0f, 1.0f, 5.0f, 5.0f );
            Assert.IsTrue( rect.Intersects( bigger ), "A rectangle should intersect a larger rectangle that encompasses it" );

            RectangleF intersected = new RectangleF( 0.8f, 2.2f, 1.5f, 3.0f );
            Assert.IsTrue( rect.Intersects( intersected ), "A rectangle should intersect a rectangle that partially encompasses it" );

            RectangleF nowhere = new RectangleF( 5.0f, 8.0f, 1.0f, 1.0f );
            Assert.IsFalse( rect.Intersects( nowhere ), "A rectangle should intersect a smaller rectangle inside it" );           
        }

        [Test]
        public void TestIntersectionAreaReturned()
        {
            RectangleF rectA = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            RectangleF rectB = new RectangleF( 0.0f, 0.0f, 3.0f, 3.0f );

            RectangleF area = RectangleF.Empty;
            bool result     = rectA.Intersects( rectB, ref area );

            Assert.IsTrue( result );

            Assert.AreEqual( 1.0f, area.X );
            Assert.AreEqual( 2.0f, area.Y );
            Assert.AreEqual( 1.5f, area.Width );
            Assert.AreEqual( 1.0f, area.Height );
        }

        [Test]
        public void TestOffsetVector()
        {
            const float horizontal = 0.5f;
            const float vertical = 1.0f;

            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );

            rect.Offset( new Vector2( horizontal, vertical ) );

            Assert.AreEqual( 1.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 1.5f, rect.Width );
            Assert.AreEqual( 3.0f, rect.Height );
        }

        [Test]
        public void TestOffsetXY()
        {
            const float horizontal = 0.5f;
            const float vertical = 1.0f;

            RectangleF rect = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );

            rect.Offset( horizontal, vertical );

            Assert.AreEqual( 1.5f, rect.X );
            Assert.AreEqual( 3.0f, rect.Y );
            Assert.AreEqual( 1.5f, rect.Width );
            Assert.AreEqual( 3.0f, rect.Height );
        }

        [Test]
        public void TestEquals()
        {
            RectangleF rect  = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            RectangleF equal = new RectangleF( rect );
            RectangleF notA  = new RectangleF( rect.X + 0.00001f, rect.Y, rect.Width, rect.Height );
            RectangleF notB  = new RectangleF( rect.X, rect.Y + 0.00001f, rect.Width, rect.Height );
            RectangleF notC  = new RectangleF( rect.X, rect.Y, rect.Width + 0.00001f, rect.Height );
            RectangleF notD  = new RectangleF( rect.X, rect.Y, rect.Width, rect.Height + 0.00001f );

            Assert.IsTrue( rect.Equals( rect ), "Rect should always equal itself" );
            Assert.IsTrue( rect.Equals( equal ), "Rect should equal another rect with the same values" );

            Assert.IsFalse( rect.Equals( notA ), "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect.Equals( notB ), "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect.Equals( notC ), "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect.Equals( notD ), "Rect should equal another rect with dissimiliar values" );

            Assert.IsFalse( rect.Equals( null ), "Rect should never equal null" );
            Assert.IsFalse( rect.Equals( new Object() ), "Rect should never equal object of another type" );
        }

        [Test]
        public void TestEqualsOperator()
        {
            RectangleF rect  = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            RectangleF equal = new RectangleF( rect );
            RectangleF notA  = new RectangleF( rect.X + 0.00001f, rect.Y, rect.Width, rect.Height );
            RectangleF notB  = new RectangleF( rect.X, rect.Y + 0.00001f, rect.Width, rect.Height );
            RectangleF notC  = new RectangleF( rect.X, rect.Y, rect.Width + 0.00001f, rect.Height );
            RectangleF notD  = new RectangleF( rect.X, rect.Y, rect.Width, rect.Height + 0.00001f );

            Assert.IsTrue( rect == equal, "Rect should equal another rect with the same values" );

            Assert.IsFalse( rect == notA, "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect == notB, "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect == notC, "Rect should equal another rect with dissimiliar values" );
            Assert.IsFalse( rect == notD, "Rect should equal another rect with dissimiliar values" );
        }

        [Test]
        public void TestNotEqualsOperator()
        {
            RectangleF rect  = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            RectangleF equal = new RectangleF( rect );
            RectangleF notA  = new RectangleF( rect.X + 0.00001f, rect.Y, rect.Width, rect.Height );
            RectangleF notB  = new RectangleF( rect.X, rect.Y + 0.00001f, rect.Width, rect.Height );
            RectangleF notC  = new RectangleF( rect.X, rect.Y, rect.Width + 0.00001f, rect.Height );
            RectangleF notD  = new RectangleF( rect.X, rect.Y, rect.Width, rect.Height + 0.00001f );

            Assert.IsFalse( rect != equal, "Rect should equal another rect with the same values" );

            Assert.IsTrue( rect != notA, "Rect should equal another rect with dissimiliar values" );
            Assert.IsTrue( rect != notB, "Rect should equal another rect with dissimiliar values" );
            Assert.IsTrue( rect != notC, "Rect should equal another rect with dissimiliar values" );
            Assert.IsTrue( rect != notD, "Rect should equal another rect with dissimiliar values" );
        }

        [Test]
        public void TestToString()
        {
            RectangleF rect  = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            Assert.AreEqual( "{ x = 1, y = 2, w = 1.5, h = 3 }", rect.ToString() );
        }

        [Test]
        public void TestHashCode()
        {
            RectangleF rect  = new RectangleF( 1.0f, 2.0f, 1.5f, 3.0f );
            RectangleF equal = new RectangleF( rect );
            RectangleF notA  = new RectangleF( rect.X + 0.00001f, rect.Y, rect.Width, rect.Height );
            RectangleF notB  = new RectangleF( rect.X, rect.Y + 0.00001f, rect.Width, rect.Height );
            RectangleF notC  = new RectangleF( rect.X, rect.Y, rect.Width + 0.00001f, rect.Height );
            RectangleF notD  = new RectangleF( rect.X, rect.Y, rect.Width, rect.Height + 0.00001f );

            Assert.AreEqual( rect.GetHashCode(), rect.GetHashCode(), "Rect should always have same hash code" );
            Assert.AreEqual( rect.GetHashCode(), equal.GetHashCode(), "Rects with identical values should have same hash code" );

            Assert.AreNotEqual( rect.GetHashCode(), notA.GetHashCode(), "Rects with different values should not have the same hash code" );
            Assert.AreNotEqual( rect.GetHashCode(), notB.GetHashCode(), "Rects with different values should not have the same hash code" );
            Assert.AreNotEqual( rect.GetHashCode(), notC.GetHashCode(), "Rects with different values should not have the same hash code" );
            Assert.AreNotEqual( rect.GetHashCode(), notD.GetHashCode(), "Rects with different values should not have the same hash code" );
        }
    }
}
