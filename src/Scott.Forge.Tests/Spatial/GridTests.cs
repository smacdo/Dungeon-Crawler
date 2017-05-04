using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Tilemaps;

namespace Scott.Forge.Tests.Spatial
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void Create_New_Empty_Grid()
        {
            var grid = new Grid<int>(3, 2);

            Assert.AreEqual(3, grid.Cols);
            Assert.AreEqual(2, grid.Rows);
            Assert.IsTrue(grid.All((v => v == default(int))));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Create_New_Grid_With_Zero_Cols_Throws_Exception()
        {
            var grid = new Grid<int>(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Create_New_Grid_With_Zero_Rows_Throws_Exception()
        {
            var grid = new Grid<int>(1, 0);
        }

        [TestMethod]
        public void Copy_Constructor_Deep_Copies_Grid()
        {
            var original = new Grid<int>(3, 2);

            original[1, 1] = 5;
            original[2, 0] = 6;

            var copied = new Grid<int>(original);

            // Test values were copied as expected.
            CollectionAssert.AreEqual(original.ToList(), copied.ToList());
            Assert.AreEqual(5, copied[1, 1]);
            Assert.AreEqual(6, copied[2, 0]);

            // Test that changing copy does not change original.
            copied[0, 1] = 42;
            Assert.AreEqual(42, copied[0, 1]);
            Assert.AreNotEqual(42, original[0, 1]);

            // Test properties equal.
            Assert.AreEqual(3, copied.Cols);
            Assert.AreEqual(2, copied.Rows);
        }

        [TestMethod]
        public void Count_Reflects_Width_And_Width()
        {
            var a = new Grid<int>(5, 2);
            Assert.AreEqual(10, a.Count);

            var b = new Grid<int>(3, 4);
            Assert.AreEqual(12, b.Count);

            b.Resize(10, 4);
            Assert.AreEqual(40, b.Count);
        }

        [TestMethod]
        public void Can_Get_And_Set_Cells_In_Grid_Using_Index_Operator()
        {
            var grid = new Grid<int>(2, 3);
            Assert.AreEqual(0, grid[0, 0]);
            Assert.AreEqual(0, grid[1, 2]);

            grid[0, 0] = 42;
            Assert.AreEqual(42, grid[0, 0]);

            grid[1, 2] = 5;
            Assert.AreEqual(42, grid[0, 0]);
            Assert.AreEqual(5, grid[1, 2]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Get_Out_Of_Bounds_Col__With_Indexer_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            var x = grid[2, 0];

            Assert.IsFalse(x == 0);     // This should not be tested but is needed to prevent warning.
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Get_Out_Of_Bounds_Row_With_Indexer_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            var x = grid[0, 3];

            Assert.IsFalse(x == 0);     // This should not be tested but is needed to prevent warning.
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Set_Out_Of_Bounds_Col_With_Indexer_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid[-1, 0] = 42;
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Set_Out_Of_Bounds_Row_With_Indexer_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid[0, -1] = 42;
        }

        [TestMethod]
        public void Grid_Storage_Is_Row_Major()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Row major would be: [1 2 3 4 5 6].
            CollectionAssert.AreEqual(
                new List<int>() { 1, 2, 3, 4, 5, 6 },
                grid.ToList());
        }

        [TestMethod]
        public void Is_Synchronized_Always_Returns_False()
        {
            var grid = new Grid<int>(1, 1);
            Assert.IsFalse(grid.IsSynchronized);
        }

        [TestMethod]
        public void Clear_Resets_All_Values()
        {
            var grid = new Grid<int>(3, 2);

            // Set a few values to non-zero.
            grid[1, 1] = 5;
            grid[2, 0] = 6;

            Assert.IsFalse(grid.All(v => v == 0));

            // Clear and test all values are zero.
            grid.Clear();
            Assert.IsTrue(grid.All(v => v == 0));
        }

        [TestMethod]
        public void Clear_Does_Not_Increment_Version()
        {
            var grid = new Grid<int>(3, 2);
            Assert.AreEqual(0, grid.Version);

            grid.Clear();
            Assert.AreEqual(0, grid.Version);
        }

        [TestMethod]
        public void Grid_Copies_To_External_Array()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Copy to external array with expected values [1 2 3 4 5 6] and [0 1 2 3 4 5 6].
            var first = new int[6];
            var second = new int[7];

            grid.CopyTo(first, 0);
            grid.CopyTo(second, 1);

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, first);
            CollectionAssert.AreEqual(new int[] { 0, 1, 2, 3, 4, 5, 6 }, second);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Grid_Copy_To_Array_Throws_Exception_If_Array_Is_Null()
        {
            var grid = new Grid<int>(2, 3);
            grid.CopyTo(null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Grid_Copy_To_Array_Throws_Exception_If_Start_Index_Less_Than_Zero()
        {
            var grid = new Grid<int>(2, 3);
            grid.CopyTo(new int[6], -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Grid_Copy_To_Array_Throws_Exception_If_Final_Index_Out_Of_Bounds()
        {
            var grid = new Grid<int>(2, 3);
            grid.CopyTo(new int[6], 1);
        }

        [TestMethod]
        public void Grid_Contains_Returns_True_If_Value_Is_In_Grid()
        {
            var grid = new Grid<int>(2, 3);
            Assert.IsFalse(grid.Contains(42));

            grid[1, 0] = 42;
            Assert.IsTrue(grid.Contains(42));

            grid[1, 0] = 0;
            Assert.IsFalse(grid.Contains(42));
        }

        [TestMethod]
        public void Grid_Contains_Returns_True_If_Value_Matches_Custom_Comparer()
        {
            // Use custom inverted comparer which returns the OPPOSITE of a normal comparer. For example the
            // custom comparer 0 == 0 is false, and 1 == 0 is true.   
            var grid = new Grid<int>(2, 3);
            Assert.IsTrue(grid.Contains(42, new CustomInvertedComparer()));

            grid[1, 0] = 42;
            Assert.IsTrue(grid.Contains(42, new CustomInvertedComparer()));

            grid[1, 0] = 0;
            Assert.IsTrue(grid.Contains(42, new CustomInvertedComparer()));
        }

        [TestMethod]
        public void Enumerate_Row_Returns_Cells_In_Given_Row()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Check rows.
            CollectionAssert.AreEqual(
                new int[] { 1, 2 },
                new List<int>(ToList(grid.EnumerateRow(0))));

            CollectionAssert.AreEqual(
                new int[] { 3, 4 },
                new List<int>(ToList(grid.EnumerateRow(1))));

            CollectionAssert.AreEqual(
                new int[] { 5, 6 },
                new List<int>(ToList(grid.EnumerateRow(2))));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Row_Throws_Excepton_If_Row_Is_Invalid()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateRow(3);
        }

        [TestMethod]
        public void Enumerate_Col_Returns_Cells_In_Given_Row()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Check rows.
            CollectionAssert.AreEqual(
                new int[] { 1, 3, 5 },
                new List<int>(ToList(grid.EnumerateColumn(0))));

            CollectionAssert.AreEqual(
                new int[] { 2, 4, 6 },
                new List<int>(ToList(grid.EnumerateColumn(1))));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Row_Throws_Excepton_If_Col_Is_Invalid()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateColumn(3);
        }

        [TestMethod]
        public void Enumerate_Rectangle_Can_Enumerate_Subregions()
        {
            var grid = new Grid<int>(3, 4);

            // Set up grid to be:
            //  [1  2  3
            //   4  5  6
            //   7  8  9
            //   10 11 12]
            grid[0, 0] = 1;  grid[1, 0] = 2;  grid[2, 0] = 3;
            grid[0, 1] = 4;  grid[1, 1] = 5;  grid[2, 1] = 6;
            grid[0, 2] = 7;  grid[1, 2] = 8;  grid[2, 2] = 9;
            grid[0, 3] = 10; grid[1, 3] = 11; grid[2, 3] = 12;

            // Enumerate entire grid.
            CollectionAssert.AreEqual(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
                new List<int>(ToList(grid.EnumerateRectangle(0, 0, 3, 4))));

            // Enumerate nothing.
            CollectionAssert.AreEqual(
                new int[] {},
                new List<int>(ToList(grid.EnumerateRectangle(0, 0, 0, 0))));

            // Enumerate subrect.
            CollectionAssert.AreEqual(
                new int[] {4, 5, 7, 8, 10, 11},
                new List<int>(ToList(grid.EnumerateRectangle(0, 1, 2, 3))));

            CollectionAssert.AreEqual(
                new int[] { 8, 9, 11, 12 },
                new List<int>(ToList(grid.EnumerateRectangle(1, 2, 2, 2))));
        }

        [TestMethod]
        public void Enumerate_Rect_With_Negative_One_Rows_And_Cols_Uses_Remaining_Rows_And_Cols()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;
            
            // Whole rect.
            CollectionAssert.AreEqual(
                new int[] { 1, 2, 3, 4, 5, 6 },
                new List<int>(ToList(grid.EnumerateRectangle(0, 0, -1, -1))));

            // Sub rect.
            CollectionAssert.AreEqual(
                new int[] { 6 },
                new List<int>(ToList(grid.EnumerateRectangle(1, 2, -1, -1))));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Rect_Throws_Exception_If_Col_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateRectangle(2, 2, -1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Rect_Throws_Exception_If_Row_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateRectangle(1, 3, -1, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Rect_Throws_Exception_If_Col_Count_Is_Excessive()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateRectangle(0, 0, 3, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Enumerate_Rect_Throws_Exception_If_Row_Count_Is_Excessive()
        {
            var grid = new Grid<int>(2, 3);
            grid.EnumerateRectangle(0, 0, 1, 4);
        }

        [TestMethod]
        public void Find_Returns_Location_Of_First_Matching_Value()
        {
            var grid = new Grid<int>(2, 3);
            int x = -1, y = -1;

            Assert.IsFalse(grid.Find(42, ref x, ref y));
            Assert.AreEqual(-1, x);
            Assert.AreEqual(-1, y);

            grid[0, 1] = 42;

            Assert.IsTrue(grid.Find(42, ref x, ref y));
            Assert.AreEqual(0, x);
            Assert.AreEqual(1, y);

            grid[1, 1] = 42;

            Assert.IsTrue(grid.Find(42, ref x, ref y));
            Assert.AreEqual(0, x);
            Assert.AreEqual(1, y);

            grid[1, 0] = 42;

            Assert.IsTrue(grid.Find(42, ref x, ref y));
            Assert.AreEqual(1, x);
            Assert.AreEqual(0, y);
        }

        [TestMethod]
        public void Find_Returns_Location_Of_First_Matching_Value_Using_Custom_Comparer()
        {
            // TOD: Test this.
        }

        [TestMethod]
        public void For_Each_Visits_All_Cells_In_Order()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Visit all cells in correct order.
            var cells = new List<int>();

            grid.ForEach((int v, int x, int y) =>
            {
                cells.Add(v);
            });

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5, 6 }, cells);
        }

        [TestMethod]
        public void For_Each_Can_Visit_Subregions()
        {
            var grid = new Grid<int>(3, 4);

            // Set up grid to be:
            //  [1  2  3
            //   4  5  6
            //   7  8  9
            //   10 11 12]
            grid[0, 0] = 1; grid[1, 0] = 2; grid[2, 0] = 3;
            grid[0, 1] = 4; grid[1, 1] = 5; grid[2, 1] = 6;
            grid[0, 2] = 7; grid[1, 2] = 8; grid[2, 2] = 9;
            grid[0, 3] = 10; grid[1, 3] = 11; grid[2, 3] = 12;

            // Enumerate entire grid.
            var cells = new List<int>();
            grid.ForEach(0, 0, 3, 4, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, cells);

            // Enumerate nothing.
            cells.Clear();
            grid.ForEach(0, 0, 0, 0, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int>(), cells);

            // Enumerate subrect.
            cells.Clear();
            grid.ForEach(0, 1, 2, 3, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int>() { 4, 5, 7, 8, 10, 11 }, cells);

            cells.Clear();
            grid.ForEach(1, 2, 2, 2, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int>() { 8, 9, 11, 12 }, cells);
        }

        [TestMethod]
        public void For_Each_Negative_One_Rows_And_Cols_Uses_Remaining_Rows_And_Cols()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Whole rect.
            var cells = new List<int>();
            grid.ForEach(0, 0, -1, -1, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int>() { 1, 2, 3, 4, 5, 6 }, cells);

            // Sub rect.
            cells.Clear();
            grid.ForEach(1, 2, -1, -1, (int v, int x, int y) => { cells.Add(v); });

            CollectionAssert.AreEqual(new List<int>() { 6 }, cells);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void For_Each_Throws_Exception_If_Col_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.ForEach(2, 2, -1, -1, (int v, int x, int y) => {});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void For_Each_Throws_Exception_If_Row_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.ForEach(1, 3, -1, -1, (int v, int x, int y) => {});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void For_Each_Throws_Exception_If_Col_Count_Is_Excessive()
        {
            var grid = new Grid<int>(2, 3);
            grid.ForEach(0, 0, 3, 1, (int v, int x, int y) => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void For_Each_Throws_Exception_If_Row_Count_Is_Excessive()
        {
            var grid = new Grid<int>(2, 3);
            grid.ForEach(0, 0, 1, 4, (int v, int x, int y) => { });
        }

        [TestMethod]
        public void Fill_Visits_Cells_In_Order()
        {
            var grid = new Grid<int>(2, 3);

            // Grid starts empty.
            Assert.IsTrue(grid.All((v => v == 0)));

            // Fill grid to be:
            //  [0  1
            //   10 11
            //   20 21]
            grid.Fill((Grid<int> g, int x, int y) => { return y * 10 + x; });
            
            CollectionAssert.AreEqual(new List<int> { 0, 1, 10, 11, 20, 21 }, grid.ToList());
        }

        [TestMethod]
        public void Can_Fill_Subregions()
        {
            var grid = new Grid<int>(3, 4);

            // Set up grid to be:
            //  [0  2  3
            //   4  5  6
            //   7  8  9
            //   10 11 12]
            grid[0, 0] = 1; grid[1, 0] = 2; grid[2, 0] = 3;
            grid[0, 1] = 4; grid[1, 1] = 5; grid[2, 1] = 6;
            grid[0, 2] = 7; grid[1, 2] = 8; grid[2, 2] = 9;
            grid[0, 3] = 10; grid[1, 3] = 11; grid[2, 3] = 12;

            // Fill entire grid to be:
            //  [0  1  2
            //   10 11 12
            //   20 21 22
            //   30 31 32]
            grid.Fill(0, 0, 3, 4, (Grid<int> g, int x, int y) => { return y * 10 + x; });
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 10, 11, 12, 20, 21, 22, 30, 31, 32 }, grid.ToList());

            // Fill nothing, result is same as previous grid since nothing changed.
            grid.Fill(0, 0, 0, 0, (Grid<int> g, int x, int y) => { return y * 10 + x; });
            CollectionAssert.AreEqual(new List<int> { 0, 1, 2, 10, 11, 12, 20, 21, 22, 30, 31, 32 }, grid.ToList());

            // Change subregion, new grid should look like:
            //  [0  1  2
            //   15 16 12
            //   25 36 22
            //   35 37 32]
            grid.Fill(0, 1, 2, 3, (Grid<int> g, int x, int y) => { return y * 10 + x + 5; });
            CollectionAssert.AreEqual(new List<int>() { 0, 1, 2, 15, 16, 12, 25, 26, 22, 35, 36, 32 }, grid.ToList());

            // Change another subregion, result should be:
            //  [0  1  2
            //   15 16 12
            //   25 71 72
            //   35 81 82]
            grid.Fill(1, 2, 2, 2, (Grid<int> g, int x, int y) => { return y * 10 + x + 50; });
            CollectionAssert.AreEqual(new List<int>() { 0, 1, 2, 15, 16, 12, 25, 71, 72, 35, 81, 82 }, grid.ToList());
        }

        [TestMethod]
        public void Fill_Can_Use_Negative_Rows_And_Cols_To_Fill_Remaining_Space()
        {
            var grid = new Grid<int>(2, 3);

            // Fill grid to be:
            //  [0  1
            //   10 11
            //   20 21]
            grid.Fill(0, 0, -1, -1, (Grid<int> g, int x, int y) => { return y * 10 + x; });
            CollectionAssert.AreEqual(new List<int> { 0, 1, 10, 11, 20, 21 }, grid.ToList());

            // Fill subregion to make grid be:
            // [0  1
            //  10 11
            //  20 25]
            grid.Fill(1, 2, -1, -1, (Grid<int> g, int x, int y) => { return y * 10 + x + 5; });
            CollectionAssert.AreEqual(new List<int> { 0, 1, 10, 11, 20, 26 }, grid.ToList());
        }
    
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Fill_Throws_Exception_If_Col_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.Fill(2, 2, -1, -1, (Grid<int> g, int x, int y) => { return 0; });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Fill_Throws_Exception_If_Row_Is_Negative()
        {
            var grid = new Grid<int>(2, 3);
            grid.Fill(1, 3, -1, -1, (Grid<int> g, int x, int y) => { return 0; });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Fill_Throws_Exception_If_Col_Count_Is_Excessive()
        {
            var grid = new Grid<int>(2, 3);
            grid.Fill(0, 0, 3, 1, (Grid<int> g, int x, int y) => { return 0; });
        }

        [TestMethod]
        public void Can_Get_And_Set_Cells_In_Grid()
        {
            var grid = new Grid<int>(2, 3);
            Assert.AreEqual(0, grid.Get(0, 0));
            Assert.AreEqual(0, grid.Get(1, 2));

            grid.Set(x: 0, y: 0, value: 42);
            Assert.AreEqual(42, grid.Get(0, 0));

            grid.Set(x: 1, y: 2, value: 5);
            Assert.AreEqual(42, grid.Get(0, 0));
            Assert.AreEqual(5, grid.Get(1, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Get_Out_Of_Bounds_Col_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid.Get(2, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Get_Out_Of_Bounds_Row_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid.Get(0, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Set_Out_Of_Bounds_Col_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid.Set(x: -1, y: 0, value: 42);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Set_Out_Of_Bounds_Row_Throws_Exception()
        {
            var grid = new Grid<int>(2, 3);
            grid.Set(x: 0, y: -1, value: 42);
        }

        [TestMethod]
        public void Get_Enumerator_Enumerates_In_Row_Major_Order()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            // Copy to external array with expected values [1 2 3 4 5 6] and [0 1 2 3 4 5 6].
            var a = ToList(grid.GetEnumerator());
            var b = ToList(((IEnumerable) grid).GetEnumerator());

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, a);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, b);
        }

        [TestMethod]
        public void Resize_Adds_Default_Elements_When_Expanding()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, grid.ToList());

            // Expand grid to be: (4 cols, 3 rows).
            //  [1 2 0 0
            //   3 4 0 0
            //   5 6 0 0]
            grid.Resize(4, 3);
            CollectionAssert.AreEqual(new int[] { 1, 2, 0, 0, 3, 4, 0, 0, 5, 6, 0, 0 }, grid.ToList());

            // Expand grid to be: (4 cols, 4 rows).
            //  [1 2 0 0
            //   3 4 0 0
            //   5 6 0 0
            //   0 0 0 0]
            grid.Resize(4, 4);
            CollectionAssert.AreEqual(
                new int[] { 1, 2, 0, 0, 3, 4, 0, 0, 5, 6, 0, 0, 0, 0, 0, 0 },
                grid.ToList());

            // Expand grid to be: (5 cols, 5 rows).
            //  [1 2 0 0 0
            //   3 4 0 0 0
            //   5 6 0 0 0
            //   0 0 0 0 0
            //   0 0 0 0 0]
            grid.Resize(5, 5);
            CollectionAssert.AreEqual(
                new int[]
                {
                    1, 2, 0, 0, 0,
                    3, 4, 0, 0, 0,
                    5, 6, 0, 0, 0,
                    0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0,
                },
                grid.ToList());
        }

        [TestMethod]
        public void Resize_And_Shrink()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, grid.ToList());

            // Shrink grid to:
            //  [1]
            grid.Resize(1, 1);
            CollectionAssert.AreEqual(new int[] { 1 }, grid.ToList());
        }

        [TestMethod]
        public void Resize_To_Same_Size()
        {
            var grid = new Grid<int>(2, 3);

            // Set up grid to be:
            //  [1 2
            //   3 4
            //   5 6]
            grid[0, 0] = 1; grid[1, 0] = 2;
            grid[0, 1] = 3; grid[1, 1] = 4;
            grid[0, 2] = 5; grid[1, 2] = 6;

            grid.Resize(2, 3);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, grid.ToList());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Resize_Throws_Exception_If_Cols_Zero()
        {
            var grid = new Grid<int>(2, 3);
            grid.Resize(0, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Resize_Throws_Exception_If_Rows_Zero()
        {
            var grid = new Grid<int>(2, 3);
            grid.Resize(2, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Resizes_Increments_Version()
        {
            var grid = new Grid<int>(2, 3);
            Assert.AreEqual(0, grid.Version);

            grid.Resize(2, 0);
            Assert.AreEqual(1, grid.Version);
        }

        private List<T> ToList<T>(IEnumerator<T> enumerator)
        {
            var result = new List<T>();

            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }

            return result;
        }

        private List<int> ToList(IEnumerator enumerator)
        {
            var result = new List<int>();

            while (enumerator.MoveNext())
            {
                result.Add((int) enumerator.Current);
            }

            return result;
        }

        private class CustomInvertedComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x != y;
            }

            public int GetHashCode(int obj)
            {
                throw new NotImplementedException();
            }
        }

    }
}
