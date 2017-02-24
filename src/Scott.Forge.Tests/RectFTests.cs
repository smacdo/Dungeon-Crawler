using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scott.Forge.Tests
{
    [TestClass]
    public class RectFTests
    {
        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void ValueConstructorSetsProperties()
        {
            var a = new RectF(3.0f, 5.0f, 20.0f, 15.0f);

            Assert.AreEqual(3.0f, a.X);
            Assert.AreEqual(5.0f, a.Y);
            Assert.AreEqual(20.0f, a.Width);
            Assert.AreEqual(15.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void ValueConstructorNegativeWidthThrowsException()
        {
            var a = new RectF(3.0f, 5.0f, -20.0f, 15.0f);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void ValueConstructorNegativeHeightThrowsException()
        {
            var a = new RectF(3.0f, 5.0f, 20.0f, -15.0f);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void TopLeftAndSizeConstructorSetsProperties()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new SizeF(20.0f, 15.0f));

            Assert.AreEqual(3.0f, a.X);
            Assert.AreEqual(5.0f, a.Y);
            Assert.AreEqual(20.0f, a.Width);
            Assert.AreEqual(15.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void TopLeftAndSizeConstructorNegativeWidthThrowsException()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new SizeF(-20.0f, 15.0f));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void TopLeftAndSizeConstructorNegativeHeightThrowsException()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new SizeF(20.0f, -15.0f));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void TopLeftAndBottomRightConstructorSetsProperties()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new Vector2(23.0f, 20.0f));

            Assert.AreEqual(3.0f, a.X);
            Assert.AreEqual(5.0f, a.Y);
            Assert.AreEqual(20.0f, a.Width);
            Assert.AreEqual(15.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void TopLeftAndBottomRightConstructorNegativeWidthThrowsException()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new Vector2(0.0f, 20.0f));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void TopLeftAndBottomRightConstructorNegativeHeightThrowsException()
        {
            var a = new RectF(new Vector2(3.0f, 5.0f), new Vector2(23.0f, 0.0f));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void CopyConstructor()
        {
            var a = new RectF(new RectF(3.0f, 5.0f, 20.0f, 15.0f));

            Assert.AreEqual(3.0f, a.X);
            Assert.AreEqual(5.0f, a.Y);
            Assert.AreEqual(20.0f, a.Width);
            Assert.AreEqual(15.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void EmptyRectHasZeroWidthAndHeight()
        {
            Assert.IsTrue(RectF.Empty.IsEmpty);
            Assert.AreEqual(0.0f, RectF.Empty.Width);
            Assert.AreEqual(0.0f, RectF.Empty.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void Area()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(0.0f, a.Area);

            a = new RectF(5.0f, 12.0f, 10.0f, 15.0f);
            Assert.AreEqual(150.0f, a.Area);

            a = new RectF(10.0f, 20.0f, 10.0f, 15.0f);
            Assert.AreEqual(150.0f, a.Area);

            a = new RectF(10.0f, 20.0f, 15.0f, 10.0f);
            Assert.AreEqual(150.0f, a.Area);

            a = new RectF(10.0f, 20.0f, 150.0f, 1.0f);
            Assert.AreEqual(150.0f, a.Area);

            a = new RectF(10.0f, 20.0f, 25.0f, 2.0f);
            Assert.AreEqual(50.0f, a.Area);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void IsEmpty()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.IsTrue(a.IsEmpty);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.IsTrue(a.IsEmpty);

            a = new RectF(0.0f, 0.0f, 1.0f, 0.0f);
            Assert.IsTrue(a.IsEmpty);

            a = new RectF(0.0f, 0.0f, 0.0f, 1.0f);
            Assert.IsTrue(a.IsEmpty);

            a = new RectF(0.0f, 0.0f, 1.0f, 1.0f);
            Assert.IsFalse(a.IsEmpty);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void GetAndSetWidth()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(5.0f, a.Width);

            // Change width.
            a.Width = 6.0f;
            Assert.AreEqual(6.0f, a.Width);

            // Make sure height, and position did not change.
            Assert.AreEqual(new Vector2(-2.0f, 3.0f), a.TopLeft);
            Assert.AreEqual(10.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetWidthNegativeThrowsException()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            a.Width = -1.0f;
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void GetAndSetX()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(-2.0f, a.X);

            // Change x.
            a.X = 6.0f;
            Assert.AreEqual(6.0f, a.X);

            // Make sure y, and size did not change.
            Assert.AreEqual(3.0f, a.Y);
            Assert.AreEqual(5.0f, a.Width);
            Assert.AreEqual(10.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void GetAndSetY()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(3.0f, a.Y);

            // Change y.
            a.Y = 6.0f;
            Assert.AreEqual(6.0f, a.Y);

            // Make sure x, and size did not change.
            Assert.AreEqual(-2.0f, a.X);
            Assert.AreEqual(5.0f, a.Width);
            Assert.AreEqual(10.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void GetAndSetHeight()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(10.0f, a.Height);

            // Change height.
            a.Height = 6.0f;
            Assert.AreEqual(6.0f, a.Height);

            // Make sure width, and position did not change.
            Assert.AreEqual(new Vector2(-2.0f, 3.0f), a.TopLeft);
            Assert.AreEqual(5.0f, a.Width);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [ExpectedException(typeof(ArgumentException))]
        public void SetHeightNegativeThrowsException()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            a.Height = -1.0f;
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void GetAndSetPosition()
        {
            var a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(-2.0f, 3.0f), a.TopLeft);

            a.TopLeft = new Vector2(3.0f, 1.0f);
            Assert.AreEqual(new Vector2(3.0f, 1.0f), a.TopLeft);

            // Make sure size did not change.
            Assert.AreEqual(5.0f, a.Width);
            Assert.AreEqual(10.0f, a.Height);
        }
        
        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void Position()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.TopLeft);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.TopLeft);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(-2.0f, 3.0f), a.TopLeft);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-2.0f, -3.0f), a.TopLeft);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void TopLeft()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.TopLeft);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.TopLeft);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(-2.0f, 3.0f), a.TopLeft);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-2.0f, -3.0f), a.TopLeft);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void TopCenter()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.TopCenter);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.TopCenter);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(0.5f, 3.0f), a.TopCenter);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.5f, -3.0f), a.TopCenter);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void TopRight()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.TopRight);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.TopRight);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(3.0f, 3.0f), a.TopRight);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.0f, -3.0f), a.TopRight);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void MidLeft()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.MidLeft);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.MidLeft);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(-2.0f, 8.0f), a.MidLeft);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-2.0f, -1.0f), a.MidLeft);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void CenterAndMidCenter()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.MidCenter);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.Center);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.MidCenter);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.Center);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(0.5f, 8.0f), a.MidCenter);
            Assert.AreEqual(new Vector2(0.5f, 8.0f), a.Center);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.5f, -1.0f), a.MidCenter);
            Assert.AreEqual(new Vector2(-1.5f, -1.0f), a.Center);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void MidRight()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.MidRight);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.MidRight);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(3.0f, 8.0f), a.MidRight);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.0f, -1.0f), a.MidRight);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void BottomLeft()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.BottomLeft);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.BottomLeft);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(-2.0f, 13.0f), a.BottomLeft);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-2.0f, 1.0f), a.BottomLeft);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void BottomCenter()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.BottomCenter);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.BottomCenter);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(0.5f, 13.0f), a.BottomCenter);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.5f, 1.0f), a.BottomCenter);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void BottomRight()
        {
            var a = new RectF(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(0.0f, 0.0f), a.BottomRight);

            a = new RectF(5.0f, 6.0f, 0.0f, 0.0f);
            Assert.AreEqual(new Vector2(5.0f, 6.0f), a.BottomRight);

            a = new RectF(-2.0f, 3.0f, 5.0f, 10.0f);
            Assert.AreEqual(new Vector2(3.0f, 13.0f), a.BottomRight);

            a = new RectF(-2.0f, -3.0f, 1.0f, 4.0f);
            Assert.AreEqual(new Vector2(-1.0f, 1.0f), a.BottomRight);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetBottomRightAndCheckPropertiesUpdated()
        {
            var a = new RectF(-5.0f, 1.0f, 0.0f, 0.0f);
            a.BottomRight = new Vector2(8.0f, 4.0f);

            Assert.AreEqual(-5.0f, a.X);
            Assert.AreEqual(1.0f, a.Y);
            Assert.AreEqual(13.0f, a.Width);
            Assert.AreEqual(3.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetCenterAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            a.Center = new Vector2(4.0f, 5.0f);

            Assert.AreEqual(-1.0f, a.X);
            Assert.AreEqual(-2.0f, a.Y);
            Assert.AreEqual(10.0f, a.Width);
            Assert.AreEqual(14.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetSizeAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            a.Size = new SizeF(4.0f, 5.0f);

            Assert.AreEqual(-2.0f, a.X);
            Assert.AreEqual(4.0f, a.Y);
            Assert.AreEqual(4.0f, a.Width);
            Assert.AreEqual(5.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetTopAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            Assert.AreEqual(4.0f, a.Top);

            a.Top = 3.0f;

            Assert.AreEqual(-2.0f, a.X);
            Assert.AreEqual(3.0f, a.Y);
            Assert.AreEqual(10.0f, a.Width);
            Assert.AreEqual(14.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetBottomAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            Assert.AreEqual(4.0f, a.Top);

            a.Bottom = 12.0f;

            Assert.AreEqual(-2.0f, a.X);
            Assert.AreEqual(4.0f, a.Y);
            Assert.AreEqual(10.0f, a.Width);
            Assert.AreEqual(8.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetLeftAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            Assert.AreEqual(-2.0f, a.Left);

            a.Left = 5.0f;

            Assert.AreEqual(5.0f, a.X);
            Assert.AreEqual(4.0f, a.Y);
            Assert.AreEqual(10.0f, a.Width);
            Assert.AreEqual(14.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void SetRightAndCheckPropertiesUpdated()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);
            Assert.AreEqual(8.0f, a.Right);

            a.Right = 5.0f;

            Assert.AreEqual(-2.0f, a.X);
            Assert.AreEqual(4.0f, a.Y);
            Assert.AreEqual(7.0f, a.Width);
            Assert.AreEqual(14.0f, a.Height);
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void ContainsPoint()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);

            // Not inside rect.
            Assert.IsFalse(a.Contains(new Vector2(-3.0f, 0.0f)));
            Assert.IsFalse(a.Contains(-3.0f, 0.0f));

            Assert.IsFalse(a.Contains(new Vector2(25.0f, 25.0f)));
            Assert.IsFalse(a.Contains(25.0f, 25.0f));

            Assert.IsFalse(a.Contains(new Vector2(1.0f, 30.0f)));
            Assert.IsFalse(a.Contains(1.0f, 30.0f));

            // Inside rect.
            Assert.IsTrue(a.Contains(new Vector2(0.0f, 5.0f)));
            Assert.IsTrue(a.Contains(0.0f, 5.0f));

            Assert.IsTrue(a.Contains(new Vector2(-1.0f, 7.0f)));
            Assert.IsTrue(a.Contains(-1.0f, 8.0f));

            // Boundaries.
            Assert.IsTrue(a.Contains(a.TopLeft));
            Assert.IsTrue(a.Contains(a.TopCenter));
            Assert.IsTrue(a.Contains(a.TopRight));
            Assert.IsTrue(a.Contains(a.MidLeft));
            Assert.IsTrue(a.Contains(a.MidCenter));
            Assert.IsTrue(a.Contains(a.MidRight));
            Assert.IsTrue(a.Contains(a.BottomLeft));
            Assert.IsTrue(a.Contains(a.BottomCenter));
            Assert.IsTrue(a.Contains(a.BottomRight));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        public void ContainsRect()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);

            // Rectangle always contains itself.
            Assert.IsTrue(a.Contains(a));

            // Rectangle contains a rectangle of the same size.
            Assert.IsTrue(a.Contains(new RectF(-2.0f, 4.0f, 10.0f, 14.0f)));

            // Rectangle contains a rect of a smaller size totally inside.
            Assert.IsTrue(a.Contains(new RectF(0, 8, 6, 7)));

            // Rectangle does not contain a rectangle larger than itself.
            Assert.IsFalse(a.Contains(new RectF(-10.0f, 4.0f, 200.0f, 200.0f)));

            // Rectangle does not contain a rectangle that intersects...
            // upper left corner intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 2, 4, 6)));

            // top intersection
            Assert.IsFalse(a.Contains(new RectF(0, 2, 4, 6)));

            // upper right corner intersection
            Assert.IsFalse(a.Contains(new RectF(6, 2, 4, 6)));

            // left intersection
            Assert.IsFalse(a.Contains(new RectF(6, 8, 4, 7)));

            // bottom left intersection
            Assert.IsFalse(a.Contains(new RectF(6, 15, 4, 5)));

            // bottom intersection
            Assert.IsFalse(a.Contains(new RectF(0, 15, 6, 6)));

            // bottom right intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 15, 4, 5)));

            // right intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 8, 4, 7)));
        }

        [TestMethod]
        [TestCategory("Forge/RectF")]
        [Ignore]
        public void IntersectsRect()
        {
            var a = new RectF(-2.0f, 4.0f, 10.0f, 14.0f);

            // Rectangle never intersects itself.
            Assert.IsFalse(a.Intersects(a));

            // Rectangle never intersects a rectangle of the same size.
            Assert.IsTrue(a.Intersects(new RectF(-2.0f, 4.0f, 10.0f, 14.0f)));

            // Rectangle contains a rect of a smaller size totally inside.
            Assert.IsTrue(a.Contains(new RectF(0, 8, 6, 7)));

            // Rectangle does not contain a rectangle larger than itself.
            Assert.IsFalse(a.Contains(new RectF(-10.0f, 4.0f, 200.0f, 200.0f)));

            // Rectangle does not contain a rectangle that intersects...
            // upper left corner intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 2, 4, 6)));

            // top intersection
            Assert.IsFalse(a.Contains(new RectF(0, 2, 4, 6)));

            // upper right corner intersection
            Assert.IsFalse(a.Contains(new RectF(6, 2, 4, 6)));

            // left intersection
            Assert.IsFalse(a.Contains(new RectF(6, 8, 4, 7)));

            // bottom left intersection
            Assert.IsFalse(a.Contains(new RectF(6, 15, 4, 5)));

            // bottom intersection
            Assert.IsFalse(a.Contains(new RectF(0, 15, 6, 6)));

            // bottom right intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 15, 4, 5)));

            // right intersection
            Assert.IsFalse(a.Contains(new RectF(-4, 8, 4, 7)));
        }
    }
}
