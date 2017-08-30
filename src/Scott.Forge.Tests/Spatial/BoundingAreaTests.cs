using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Spatial.Tests
{
    [TestClass]
    public class BoundingAreaTests
    {
        [TestMethod]
        public void CreateEmptyBoundingAreaInitsToDefaults()
        {
            var b = new BoundingArea();

            Assert.AreEqual(0.0f, b.Width);
            Assert.AreEqual(0.0f, b.Height);
            Assert.AreEqual(0.0f, b.Rotation);

            Assert.AreEqual(Vector2.Zero, b.WorldPosition);
            Assert.AreEqual(Vector2.Zero, b.Pivot);

            Assert.AreEqual(Vector2.Zero, b.UpperLeft);
            Assert.AreEqual(Vector2.Zero, b.UpperRight);
            Assert.AreEqual(Vector2.Zero, b.LowerLeft);
            Assert.AreEqual(Vector2.Zero, b.LowerRight);

            Assert.AreEqual(Vector2.Zero, b.LocalUpperLeft);
            Assert.AreEqual(Vector2.Zero, b.LocalUpperRight);
            Assert.AreEqual(Vector2.Zero, b.LocalLowerLeft);
            Assert.AreEqual(Vector2.Zero, b.LocalLowerRight);
        }

        [TestMethod]
        public void CreateBoundingRectFromRectangle()
        {
            var topLeft = new Vector2(3, 2);
            var size = new SizeF(4, 6);

            var b = new BoundingArea(new RectF(topLeft, size), 0.0f);

            Assert.AreEqual(4f, b.Width);
            Assert.AreEqual(6f, b.Height);
            Assert.AreEqual(0.0f, b.Rotation);

            Assert.AreEqual(topLeft, b.WorldPosition);
            Assert.AreEqual(Vector2.Zero, b.Pivot);

            Assert.AreEqual(topLeft, b.UpperLeft);
            Assert.AreEqual(topLeft + new Vector2(size.Width, 0.0f), b.UpperRight);
            Assert.AreEqual(topLeft + new Vector2(0.0f, size.Height), b.LowerLeft);
            Assert.AreEqual(topLeft + new Vector2(size.Width, size.Height), b.LowerRight);

            Assert.AreEqual(Vector2.Zero, b.LocalUpperLeft);
            Assert.AreEqual(new Vector2(size.Width, 0.0f), b.LocalUpperRight);
            Assert.AreEqual(new Vector2(0.0f, size.Height), b.LocalLowerLeft);
            Assert.AreEqual(new Vector2(size.Width, size.Height), b.LocalLowerRight);
        }

        [TestMethod]
        public void CreateBoundingRectWithRotation()
        {
            var topLeft = new Vector2(3, 2);
            var size = new SizeF(4, 6);
            var rotation = (float)Math.PI * 0.5f;      // 90 degrees.

            var b = new BoundingArea(new RectF(topLeft, size), rotation);

            Assert.AreEqual(4f, b.Width);
            Assert.AreEqual(6f, b.Height);
            Assert.AreEqual(rotation, b.Rotation);

            Assert.AreEqual(topLeft, b.WorldPosition);
            Assert.AreEqual(Vector2.Zero, b.Pivot);

            Assert.AreEqual(topLeft, b.UpperLeft);        // Pivot stays at same position.
            Assert.AreEqual(3, Math.Round(b.UpperRight.X));
            Assert.AreEqual(6, Math.Round(b.UpperRight.Y));
            Assert.AreEqual(-3, Math.Round(b.LowerLeft.X));
            Assert.AreEqual(2, Math.Round(b.LowerLeft.Y));
            Assert.AreEqual(-3, Math.Round(b.LowerRight.X));
            Assert.AreEqual(6, Math.Round(b.LowerRight.Y));

            Assert.AreEqual(Vector2.Zero, b.LocalUpperLeft);        // Pivot stays at same position.
            Assert.AreEqual(0, Math.Round(b.LocalUpperRight.X));
            Assert.AreEqual(4, Math.Round(b.LocalUpperRight.Y));
            Assert.AreEqual(-6, Math.Round(b.LocalLowerLeft.X));
            Assert.AreEqual(0, Math.Round(b.LocalLowerLeft.Y));
            Assert.AreEqual(-6, Math.Round(b.LocalLowerRight.X));
            Assert.AreEqual(4, Math.Round(b.LocalLowerRight.Y));
        }

        [TestMethod]
        public void CreateBoundingRectWithRotationAndPivot()
        {
            var topLeft = new Vector2(3, 2);
            var size = new SizeF(4, 6);
            var rotation = (float)Math.PI * 0.5f;      // 90 degrees.
            var pivot = new Vector2(2, 3);          // Middle.

            var b = new BoundingArea(topLeft, size, rotation, pivot);

            Assert.AreEqual(4f, b.Width);
            Assert.AreEqual(6f, b.Height);
            Assert.AreEqual(rotation, b.Rotation);

            Assert.AreEqual(topLeft, b.WorldPosition);
            Assert.AreEqual(pivot, b.Pivot);

            Assert.AreEqual(8, Math.Round(b.UpperLeft.X));
            Assert.AreEqual(3, Math.Round(b.UpperLeft.Y));
            Assert.AreEqual(8, Math.Round(b.UpperRight.X));
            Assert.AreEqual(7, Math.Round(b.UpperRight.Y));
            Assert.AreEqual(2, Math.Round(b.LowerLeft.X));
            Assert.AreEqual(3, Math.Round(b.LowerLeft.Y));
            Assert.AreEqual(2, Math.Round(b.LowerRight.X));
            Assert.AreEqual(7, Math.Round(b.LowerRight.Y));

            Assert.AreEqual(5, Math.Round(b.LocalUpperLeft.X));
            Assert.AreEqual(1, Math.Round(b.LocalUpperLeft.Y));
            Assert.AreEqual(5, Math.Round(b.LocalUpperRight.X));
            Assert.AreEqual(5, Math.Round(b.LocalUpperRight.Y));
            Assert.AreEqual(-1, Math.Round(b.LocalLowerLeft.X));
            Assert.AreEqual(1, Math.Round(b.LocalLowerLeft.Y));
            Assert.AreEqual(-1, Math.Round(b.LocalLowerRight.X));
            Assert.AreEqual(5, Math.Round(b.LocalLowerRight.Y));
        }

        [TestMethod]
        public void CheckNonIntersectingAreas()
        {
            var a = new BoundingArea(new Vector2(0, 1), new SizeF(2, 3));
            var b = new BoundingArea(new Vector2(3, 6), new SizeF(4, 2));
            var v = Vector2.Zero;

            var result = a.Intersects(b, ref v);

            Assert.IsFalse(result);
            Assert.IsTrue(v.IsZero);
        }

        [TestMethod]
        public void CheckOverlapIntersectingAreas()
        {
            var a = new BoundingArea(new Vector2(0, 1), new SizeF(2, 3));
            var b = new BoundingArea(new Vector2(1, -1), new SizeF(2, 4));
            var v = Vector2.Zero;

            var result = a.Intersects(b, ref v);

            Assert.IsTrue(result);
            Assert.AreEqual(-1, Math.Round(v.X));
            Assert.AreEqual(0, Math.Round(v.Y));
        }

        [TestMethod]
        public void CheckAdjacentIntersectingAreas()
        {
            var a = new BoundingArea(new Vector2(0, 1), new SizeF(2, 3));
            var b = new BoundingArea(new Vector2(2, 1), new SizeF(1, 2));
            var v = Vector2.Zero;

            var result = a.Intersects(b, ref v);

            Assert.IsFalse(result);
            Assert.IsTrue(v.IsZero);
        }

        [TestMethod]
        public void CheckInternalIntersectingAreas()
        {
            var a = new BoundingArea(new Vector2(5, 8), new SizeF(9, 9));
            var b = new BoundingArea(new Vector2(6, 10), new SizeF(2, 2));
            var v = Vector2.Zero;

            var result = a.Intersects(b, ref v);

            Assert.IsTrue(result);
            Assert.AreEqual(3, Math.Round(v.X));
            Assert.AreEqual(0, Math.Round(v.Y));
        }

        // TODO: Rotate the bounding area 45 degree and test if seperating axis is correct.
    }
}
