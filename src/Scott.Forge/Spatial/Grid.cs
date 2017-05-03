/*
 * Copyright 2012-2017 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

// TODO: Make sure Grid from other class has these changes before commiting.
// TODO: Add negative cols/rows to iterate for EnumerateRect, ForEach, Fill. Update bounds checks (0 and max), and
//       unit tests for both sides since this is tricky.

namespace Scott.Forge.Spatial
{
    /// <summary>
    ///  Two dimensional grid storage class.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("Cols = {Cols}, Rows = {Rows}")]
    public class Grid<TValue> : IEnumerable<TValue>
    {
        /// <summary>
        ///  Create a new grid with the given dimensions.
        /// </summary>
        /// <param name="cols">Number of columns in the grid.</param>
        /// <param name="cols">Number of rows in the grid.</param>
        public Grid(int cols, int rows)
        {
            if (cols < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cols), "Column count must be larger than zero");
            }

            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Row count must be larger than zero");
            }

            // Allocate a new array to hold tiles.
            Cells = new TValue[rows, cols];
        }

        /// <summary>
        ///  Copy constructor.
        /// </summary>
        /// <param name="copy">Grid to copy values from.</param>
        public Grid(Grid<TValue> copy)
        {
            if (copy == null)
            {
                throw new ArgumentNullException(nameof(copy));
            }

            Cells = new TValue[copy.Rows, copy.Cols];
            Array.Copy(copy.Cells, Cells, copy.Count);
        }

        /// <summary>
        ///  Get or set the array holding the grid's cell values.
        /// </summary>
        /// <remarks>
        ///  Values are stored row major, in the form of [y, x] ordering.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [DataMember(Name = "Cells", IsRequired = true, Order = 0)]
        internal TValue[,] Cells { [DebuggerStepThrough] get; [DebuggerStepThrough] private set; }

        /// <summary>
        ///  Get the number of items in this grid.
        /// </summary>
        public int Count { [DebuggerStepThrough] get { return Cols * Rows; } }

        /// <summary>
        ///  Get if this grid supports access from multiple threads.
        /// </summary>
        [DebuggerHidden]
        public bool IsSynchronized { [DebuggerStepThrough] get { return false; } }

        /// <summary>
        ///  Get the height (number of rows) of this grid.
        /// </summary>
        public int Rows { [DebuggerStepThrough] get { return Cells.GetLength(0); } }

        /// <summary>
        ///  Get the width (number of columns) of this grid.
        /// </summary>
        public int Cols { [DebuggerStepThrough] get { return Cells.GetLength(1); } }

        /// <summary>
        ///  Get or set a cell in the grid.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        public TValue this[int x, int y]
        {
            [DebuggerStepThrough] get { return Cells[y, x]; }
            [DebuggerStepThrough] set { Cells[y, x] = value; }
        }

        /// <summary>
        ///  Get or set a version value.
        /// </summary>
        /// <remarks>
        ///  This value is incremented each time a change is made to the object that would invalidate any active
        ///  enumerators. This allows the enumerator to detect changes and thrown an exception to warn the user.
        /// </remarks>
        [DebuggerHidden]
        internal int Version { get; private set; }

        /// <summary>
        ///  Set all values in the grid to a default value.
        /// </summary>
        public void Clear()
        {
            Array.Clear(Cells, 0, Count);
        }

        /// <summary>
        ///  Copy cells to an array.
        /// </summary>
        /// <param name="array">Array to copy values to.</param>
        /// <param name="startIndex">Array index to start copying values.</param>
        public void CopyTo(TValue[] array, int startIndex)
        {
            // Check parameters before attempting to copy values.
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (startIndex < 0 || startIndex + Count > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // Manually loop through cells and copy them into the destination array because C# does not support
            // copying from a two dimensional array into a one dimensional array.
            var index = startIndex;

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    array[index++] = Cells[y, x];
                }
            }
        }

        /// <summary>
        ///  Check if a value is in this grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <returns>True if the value exists in the grid, false otherwise.</returns>
        public bool Contains(TValue value)
        {
            return Contains(value, EqualityComparer<TValue>.Default);
        }

        /// <summary>
        ///  Check if a value is in this grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparer">Equality comparer to use.</param>
        /// <returns>True if the value exists in the grid, false otherwise.</returns>
        public bool Contains(TValue value, IEqualityComparer<TValue> comparer)
        {
            int x = 0, y = 0;
            return Find(value, comparer, ref x, ref y);
        }

        /// <summary>
        ///  Enumerate all values in the row in ascending column order.
        /// </summary>
        /// <param name="y">Row index.</param>
        /// <returns>Grid cell enumerator.</returns>
        public IEnumerator<TValue> EnumerateRow(int y)
        {
            return new GridEnumerator<TValue>(this, 0, y, Cols, 1);
        }

        /// <summary>
        ///  Enumerate all values in the column in ascending row order.
        /// </summary>
        /// <param name="x">Column index.</param>
        /// <returns>Grid cell enumerator.</returns>
        public IEnumerator<TValue> EnumerateColumn(int x)
        {
            return new GridEnumerator<TValue>(this, x, 0, 1, Rows);
        }

        /// <summary>
        ///  Enumerate all values in a rectangular area of the grid.
        /// </summary>
        /// <remarks>
        ///  Objects are iterated in row and then column order. So the first row's columns are iterated, then the
        ///  second row and so on.
        /// </remarks>
        /// <param name="startX">Cell x index to start from.</param>
        /// <param name="startY">Cell y index to start from.</param>
        /// <param name="cols">Columns (width) to iterate. Use -1 to iterate to the right most cell.</param>
        /// <param name="rows">Rows (height) to iterate. Use -1 to iterate to the bottom most cell.</param>
        /// <returns>Grid cell enumerator.</returns>
        public IEnumerator<TValue> EnumerateRectangle(int startX, int startY, int cols, int rows)
        {
            // Use remaining width or remaining height if either width is -1 or height is -1.
            if (cols == -1)
            {
                cols = Cols - startX;
            }

            if (rows == -1)
            {
                rows = Rows - startY;
            }

            return new GridEnumerator<TValue>(this, startX, startY, cols, rows);
        }

        /// <summary>
        ///  Find a value in the grid and return its position in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="atX">Receives the x coordinate of the cell containing the value.</param>
        /// <param name="atY">Receives the y coordinate of the cell containing the value.</param>
        public bool Find(TValue value, ref int atX, ref int atY)
        {
            return Find(value, EqualityComparer<TValue>.Default, ref atX, ref atY);
        }

        /// <summary>
        ///  Find a value in the grid and return its position in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparer">Equality comparer to use.</param>
        /// <param name="atX">Receives the x coordinate of the cell containing the value.</param>
        /// <param name="atY">Receives the y coordinate of the cell containing the value.</param>
		public bool Find(TValue value, IEqualityComparer<TValue> comparer, ref int atX, ref int atY)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer), "Comparer cannot be null");
            }

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    if (comparer.Equals(value, Cells[y, x]))
                    {
                        atX = x;
                        atY = y;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  Invoke the provided action on each cell in the grid.
        /// </summary>
        /// <remarks>
        ///  This method will iterate over every column in row order, so the first row's columns will be iterated over
        ///  and then the second and so on.
        ///  
        ///  The action's first two parameters are the X and Y position of the current cell being visited. The third
        ///  parametre is the value of the cell.
        /// </remarks>
        /// <param name="action">Action to execute.</param>
        public void ForEach(Action<TValue, int, int> action)
        {
            ForEach(0, 0, Cols, Rows, action);
        }

        /// <summary>
        ///  Invoke the provided action on each cell in the grid.
        /// </summary>
        /// <remarks>
        ///  This method will iterate over every column in row order, so the first row's columns will be iterated over
        ///  and then the second and so on.
        ///  
        ///  The action's first two parameters are the X and Y position of the current cell being visited. The third
        ///  parametre is the value of the cell.
        /// </remarks>
        /// <param name="action">Action to execute.</param>
        /// <param name="x">Cell column to start at.</param>
        /// <param name="y">Cell row to start at.</param>
        /// <param name="cols">The number of columns (width) to visit.</param>
        /// <param name="rows">The number of rows (height) to visit.</param>
        public void ForEach(int x, int y, int cols, int rows, Action<TValue, int, int> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Check if caller provided a width and height argument. If the caller used the default -1, then calculate
            // the width and height required to go from the startXY positions to the ends of the grid.
            if (cols == -1)
            {
                cols = Cols - x;
            }

            if (rows == -1)
            {
                rows = Rows - y;
            }

            // Validate arguments.
            ValidateIteratableRegionArguments(x, y, cols, rows, Cols, Rows);

            // Calculate last x and y tiles to visit by using the width/height arguments. 
            var endX = x + cols;
            var endY = y + rows;

            Debug.Assert(endX >= x && endX <= Cols);
            Debug.Assert(endY >= y && endY <= Rows);

            // Iterate each element.
            var cachedVersion = Version;

            for (var yIndex = y; yIndex < endY; yIndex++)
            {
                for (var xIndex = x; xIndex < endX; xIndex++)
                {
                    Debug.Assert(cachedVersion == Version);
                    action(Cells[yIndex, xIndex], xIndex, yIndex);
                }
            }
        }

        /// <summary>
        ///  Invoke the provided action on each cell in the grid and set the result as the new grid value.
        /// </summary>
        public void Fill(Func<Grid<TValue>, int, int, TValue> action)
        {
            Fill(0, 0, -1, -1, action);
        }

        /// <summary>
        ///  Invoke the provided action on each cell in the region and set the result as the new grid value.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="x">Cell column to start at.</param>
        /// <param name="y">Cell row to start at.</param>
        /// <param name="cols">The number of columns (width) to visit.</param>
        /// <param name="rows">The number of rows (height) to visit.</param>
        public void Fill(int x, int y, int cols, int rows, Func<Grid<TValue>, int, int, TValue> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Check if caller provided a width and height argument. If the caller used the default -1, then calculate
            // the width and height required to go from the startXY positions to the ends of the grid.
            if (cols == -1)
            {
                cols = Cols - x;
            }

            if (rows == -1)
            {
                rows = Rows - y;
            }

            // Validate arguments.
            ValidateIteratableRegionArguments(x, y, cols, rows, Cols, Rows);

            // Calculate last x and y tiles to visit by using the width/height arguments. 
            var endX = x + cols;
            var endY = y + rows;

            Debug.Assert(endX >= x && endX <= Cols);
            Debug.Assert(endY >= y && endY <= Rows);

            // Iterate each element.
            var cachedVersion = Version;

            for (var yIndex = y; yIndex < endY; yIndex++)
            {
                for (var xIndex = x; xIndex < endX; xIndex++)
                {
                    Debug.Assert(cachedVersion == Version);
                    this[xIndex, yIndex] = action(this, xIndex, yIndex);
                }
            }
        }

        /// <summary>
        ///  Get a cell in the grid.
        /// </summary>
        /// <param name="x">X position of the cell.</param>
        /// <param name="y">Y position of the cell.</param>
        /// <returns>Value of the cell.</returns>
        public TValue Get(int x, int y)
        {
            return Cells[y, x];
        }

        /// <summary>
        ///  Set a cell in the grid.
        /// </summary>
        /// <param name="x">X position of the cell.</param>
        /// <param name="y">Y position of the cell.</param>
        /// <param name="value">Value to set.</param>
		public void Set(int x, int y, TValue value)
        {
            Cells[y, x] = value;
        }

        /// <summary>
        ///  Enumerate all cells in the grid.
        /// </summary>
        /// <returns>Grid cell enumerator.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return new GridEnumerator<TValue>(this);
        }

        /// <summary>
        ///  Enumerate all cells in the grid.
        /// </summary>
        /// <returns>Grid cell enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Resize the grid to a new size.
        /// </summary>
        /// <remarks>
        ///  The grid is resized using the top left as an anchor, and expands (or shrinks) to the right and downward.
        ///  A smaller width or height will remove cells from the grid, and a larger width or height will add default
        ///  values to the grid.
        /// </remarks>
        /// <param name="newRows">New grid height.</param>
        /// <param name="newCols">New grid width.</param>
        public void Resize(int newCols, int newRows)
        {
            if (newCols < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(newCols), "New column count must be larger than zero");
            }

            if (newRows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(newRows), "New row count must be larger than zero");
            }

            var newValues = new TValue[newRows, newCols];

            var maxRowToCopy = Math.Min(Rows, newRows);
            var maxColToCopy = Math.Min(Cols, newCols);

            for (int y = 0; y < maxRowToCopy; y++)
            {
                for (int x = 0; x < maxColToCopy; x++)
                {
                    newValues[y, x] = Cells[y, x];
                }
            }

            Version++;
            Cells = newValues;
        }

        /// <summary>
        ///  Validates that a rectangular set of cells are inside the boundaries of this grid.
        /// </summary>
        /// <param name="x">Minimum x value in the selected region.</param>
        /// <param name="y">Minimum y value in the selected region.</param>
        /// <param name="cols">Number of columns in the selected region.</param>
        /// <param name="rows">Number of rows in the selected region.</param>
        /// <param name="totalCols">Total column count in the grid.</param>
        /// <param name="totalRows">Total row count in the grid.</param>
        internal static void ValidateIteratableRegionArguments(
            int x,
            int y,
            int cols,
            int rows,
            int totalCols,
            int totalRows)
        {
            if (x < 0 || x >= totalCols)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "X start index is out of bounds");
            }

            if (y < 0 || y >= totalRows)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y start index is out of bounds");
            }

            if (x + cols > totalCols)
            {
                throw new ArgumentOutOfRangeException(nameof(cols), "Columns to iterate is too large");
            }

            if (y + rows > totalRows)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows to iterate is too large");
            }
        }
    }

    /// <summary>
    ///  Grid enumerator.
    /// </summary>
    public sealed class GridEnumerator<TValue> : IEnumerator<TValue>, IEnumerator, IDisposable
    {
        private Grid<TValue> mGrid;

        private int mX = 0;
        private int mY = 0;
        private int mStartX = 0;
        private int mEndX = 0;          // Range: [mStartX, mEndX)
        private int mEndY = 0;         // Range: [startY, mEndY)
        private int mVersion = 0;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="grid">Grid to enumerate.</param>
        internal GridEnumerator(Grid<TValue> grid)
            : this(grid, 0, 0, grid.Cols, grid.Rows)
        {
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="grid">Grid to enumerate.</param>
        /// <param name="x">Minimum x position in the region.</param>
        /// <param name="y">Minimum y position in the region.</param>
        /// <param name="cols">Number of columns across to enumerate.</param>
        /// <param name="rows">Number of rows down to enumerate.</param>
        internal GridEnumerator(Grid<TValue> grid, int x, int y, int cols, int rows)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            Grid<TValue>.ValidateIteratableRegionArguments(x, y, cols, rows, grid.Cols, grid.Rows);

            mGrid = grid;
            mX = x;
            mY = y;
            mStartX = x;
            mEndX = x + cols;
            mEndY = y + rows;
            mVersion = grid.Version;

            Current = default(TValue);
        }

        /// <summary>
        ///  Get the current value.
        /// </summary>
        public TValue Current { get; private set; }

        /// <summary>
        ///  Get current value.
        /// </summary>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        ///  Dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///  Move to next enumerable value.
        /// </summary>
        /// <returns>True if there is a next value, false otherwise.</returns>
        public bool MoveNext()
        {
            if (mVersion != mGrid.Version)
            {
                throw new InvalidOperationException("Grid enumerator invalidated by change to grid");
            }

            bool isIndexValid = (mX < mEndX && mY < mEndY);

            if (isIndexValid)
            {
                Current = mGrid[mX, mY];

                if (mX + 1 == mEndX)
                {
                    mX = mStartX;
                    mY += 1;
                }
                else
                {
                    mX += 1;
                }
            }
            else
            {
                // Reached end of collection.
                mX = mEndX + 1;
                mY = mEndY + 1;

                Current = default(TValue);
            }

            return isIndexValid;
        }

        /// <summary>
        ///  Reset enumerator.
        /// </summary>
        public void Reset()
        {
            throw new NotSupportedException("Grid enumerator does not support reset");
        }
    }
}
