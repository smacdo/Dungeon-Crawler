/*
 * Copyright 2013 - 2018 Scott MacDonald
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

// TODO: Add negative cols/rows to iterate for EnumerateRect, ForEach, Fill. Update bounds checks (0 and max), and
//       unit tests for both sides since this is tricky.

namespace Forge.Spatial
{
    /// <summary>
    ///  Two dimensional grid storage class.
    /// </summary>
    /// <remarks>
    ///  Cartesian coordinate system with the origin in the top left corner. X increases to the right, and Y increases down.
    /// </remarks>
    [DebuggerDisplay("Cols = {Cols}, Rows = {Rows}")]
    public class Grid<TValue> : IGrid<TValue>
    {
        /// <summary>
        ///  Create a grid with the given dimensions and populated with the values from the enumerable collection.
        /// </summary>
        /// <remarks>
        ///  The provided collection is a flat array containing values in row major order. This means that each row is
        ///  stored consecutively in memory. There must be enough values in the enumerable to populate every cell in
        ///  the grid or an exception will be thrown.
        /// </remarks>
        /// <param name="cols">Number of cells across.</param>
        /// <param name="rows">Number of cell down.</param>
        /// <param name="input">A collection used to populate the grid.</param>
        public Grid(int cols, int rows, IEnumerable<TValue> source = null)
        {
            if (cols < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cols), "Columns must be larger than zero");
            }

            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be larger than zero");
            }

            // Allocate a new array to hold tiles.
            Cells = new TValue[rows, cols];

            // Only copy values if an enumerable collection was provided.
            if (source != null)
            {
                // Iterate through the collection of input values and assign these values into the tile grid.
                int x = 0;
                int y = 0;

                foreach (var v in source)
                {
                    if (x == cols)
                    {
                        x = 0;
                        y++;
                    }
                    else if (y == rows)
                    {
                        break;  // more elements not needed, done copying tiles.
                    }

                    Cells[y, x] = v;
                    x++;
                }

                // Verify that enough values were copied to fully populate the grid.
                if (x != cols && y != rows)
                {
                    throw new ArgumentException("Not enough values to populate grid", nameof(source));
                }
            }
        }

        /// <summary>
        ///  Create a new grid using the provided two dimensional array.
        /// </summary>
        /// <remarks>
        ///  The array must be row major where the first rank is height and the secon rank is width. When accessing
        ///  callers use the [y, x] notation rather than [x, y].
        /// </remarks>
        /// <param name="source">Array to copy values from.</param>
        public Grid(TValue[,] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var rows = source.GetLength(0);
            var cols = source.GetLength(1);

            Cells = new TValue[rows, cols];
            Array.Copy(source, Cells, rows * cols);
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
        public int Count
        {
            [DebuggerStepThrough]
            get { return Cols * Rows; }
        }

        /// <summary>
        ///  Get if this grid supports access from multiple threads.
        /// </summary>
        [DebuggerHidden]
        public bool IsSynchronized
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        ///  Get the height (number of rows) of this grid.
        /// </summary>
        public int Rows
        {
            [DebuggerStepThrough]
            get { return Cells.GetLength(0); }
        }

        /// <summary>
        ///  Get the width (number of columns) of this grid.
        /// </summary>
        public int Cols
        {
            [DebuggerStepThrough]
            get { return Cells.GetLength(1); }
        }
        
        /// <summary>
        ///  Get or set a value in the grid.
        /// </summary>
        /// <param name="x">X index of the value.</param>
        /// <param name="y">Y index of the value.</param>
        
        public TValue this[int x, int y]
        {
            [DebuggerStepThrough] get { return Cells[y, x]; }
            [DebuggerStepThrough] set { Cells[y, x] = value; }
        }

        /// <summary>
        ///  Get or set a value in the grid.
        /// </summary>
        /// <param name="p">Cell index.</param>
        public TValue this[Point2 index]
        {
            [DebuggerStepThrough] get { return Cells[index.Y, index.X]; }
            [DebuggerStepThrough] set { Cells[index.Y, index.X] = value; }
        }

        /// <summary>
        ///  Get or set a modification tracker value.
        /// </summary>
        /// <remarks>
        ///  This value is incremented each time a change is made to the object that would invalidate any active
        ///  enumerators. This allows the enumerator to detect changes and thrown an exception to warn the user.
        /// </remarks>
        [DebuggerHidden]
        public int Version { get; private set; }

        /// <summary>
        ///  Get an object reference to synchronize on when using this object from multiple threads.
        /// </summary>
        /// <remarks>
        ///  The same reference will always be returned for the same grid and this will never be the same as the
        ///  reference returned for a different grid (even for a clone).
        /// </remarks>
        [DebuggerHidden]
        public object SyncRoot { get; private set; } = new object();

        /// <summary>
        ///  Clear all values in the grid to their default value. The grid size is not changed.
        /// </summary>
        public void Clear()
        {
            Array.Clear(Cells, 0, Count);
        }

        /// <summary>
        ///  Copy values to an array.
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
        /// <param name="comparer">Equality comparer to use.</param>
        /// <returns>True if the value exists in the grid, false otherwise.</returns>
        public bool Contains(TValue value, IEqualityComparer<TValue> comparer = null)
        {
            var index = IndexOf(value, comparer ?? EqualityComparer<TValue>.Default);
            return index.HasValue;
        }

        /// <summary>
        ///  Enumerate all values in a row.
        /// </summary>
        /// <remarks>
        ///  Values are visited in ascending column order (eg smaller column indices are visted earlier).
        /// </remarks>
        /// <param name="y">Row index.</param>
        /// <returns>Grid cell enumerator.</returns>
        public IReadOnlyGridEnumerator<TValue> EnumerateRow(int y)
        {
            return new IReadOnlyGridEnumerator<TValue>(this, 0, y, Cols, 1);
        }

        /// <summary>
        ///  Enumerate all values in the column.
        /// </summary>
        /// <remarks>
        ///  Values are visited in ascending row order (eg smaller row indices are visted earlier).
        /// </remarks>
        /// <param name="x">Column index.</param>
        /// <returns>Grid cell enumerator.</returns>
        public IReadOnlyGridEnumerator<TValue> EnumerateColumn(int x)
        {
            return new IReadOnlyGridEnumerator<TValue>(this, x, 0, 1, Rows);
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
        public IReadOnlyGridEnumerator<TValue> EnumerateRectangle(int startX, int startY, int cols, int rows)
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

            return new IReadOnlyGridEnumerator<TValue>(this, startX, startY, cols, rows);    // TODO: use named arguments
        }

        /// <summary>
        ///  Get a value in the grid.
        /// </summary>
        /// <remarks>
        ///  This method performs bounds checking, unlike the subscript operator. Calling Get with an invalid position
        ///  will throw an ArgumentOutOfRange exception.
        /// </remarks>
        /// <param name="x">X position of the cell.</param>
        /// <param name="y">Y position of the cell.</param>
        /// <returns>Value from the grid cell.</returns>
        public TValue Get(int x, int y)
        {
            if (x < 0 || x >= Cols)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return Cells[y, x];
        }

        /// <summary>
        ///  Get a value in the grid.
        /// </summary>
        /// <remarks>
        ///  This method performs bounds checking, unlike the subscript operator. Calling Get with an invalid position
        ///  will throw an ArgumentOutOfRange exception.
        /// </remarks>
        /// <param name="index">Index of the value in the grid.</param>
        public TValue Get(Point2 index)
        {
            return Get(index.X, index.Y);
        }

        /// <summary>
        ///  Set a value in the grid.
        /// </summary>
        /// <remarks>
        ///  This method performs bounds checking, unlike the subscript operator. Calling Set with an invalid position
        ///  will throw an ArgumentOutOfRange exception.
        /// </remarks>
        /// <param name="x">X position of the cell.</param>
        /// <param name="y">Y position of the cell.</param>
        /// <param name="value">Value to put in the cell.</param>
		public void Set(int x, int y, TValue value)
        {
            if (x < 0 || x >= Cols)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= Rows)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            Cells[y, x] = value;
        }

        /// <summary>
        ///  Set a value in the grid.
        /// </summary>
        /// <remarks>
        ///  This method performs bounds checking, unlike the subscript operator. Calling Set with an invalid position
        ///  will throw an ArgumentOutOfRange exception.
        /// </remarks>
        /// <param name="index">Index of the value in the grid.</param>
        /// <param name="value">Value to put in the cell.</param>
        public void Set(Point2 index, TValue value)
        {
            Set(index.X, index.Y, value);
        }

        /// <summary>
        ///  Enumerate all values in the grid.
        /// </summary>
        /// <returns>Grid cell enumerator.</returns>
        public IReadOnlyGridEnumerator<TValue> GetEnumerator()
        {
            return new IReadOnlyGridEnumerator<TValue>(this);
        }

        /// <summary>
        ///  Enumerate all values in the grid.
        /// </summary>
        /// <returns>Grid cell enumerator.</returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///  Enumerate all values in the grid.
        /// </summary>
        /// <returns>Grid cell enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///  Get the index of a value in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <returns>The index of the value in the grid.</returns>
        public Point2? IndexOf(TValue value)
        {
            return IndexOf(value, EqualityComparer<TValue>.Default);
        }

        /// <summary>
        ///  Get the index of a value in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparer">Optional equality comparer to use.</param>
        /// <returns>The index of the value in the grid.</returns>
        public Point2? IndexOf(TValue value, IEqualityComparer<TValue> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<TValue>.Default;

            for (int y = 0; y < Rows; ++y)
            {
                for (int x = 0; x < Cols; ++x)
                {
                    if (comparer.Equals(value, Cells[y, x]))
                    {
                        return new Point2(x, y);
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///  Resize the grid.
        /// </summary>
        /// <remarks>
        ///  The grid is resized using the top left as an anchor, and expands (or shrinks) to the right and downward.
        ///  A smaller width or height will remove cells from the grid, and a larger width or height will add default
        ///  values to the grid.
        /// </remarks>
        /// <param name="newHeight">New grid height.</param>
        /// <param name="newWidth">New grid width.</param>
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
    }
}
