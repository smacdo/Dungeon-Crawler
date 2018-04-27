/*
 * Copyright 2018 Scott MacDonald
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

namespace Forge.Spatial
{
    /// <summary>
    ///  Two dimensional grid.
    /// </summary>
    /// <remarks>
    ///  Cartesian coordinate system with the origin in the top left corner. X increases to the right, and Y increases down.
    /// </remarks>
    public interface IReadOnlyGrid<TValue> : IEnumerable<TValue>
    {
        /// <summary>
        ///  Get the number of values in this grid.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///  Get the height (number of rows) of this grid.
        /// </summary>
        int Rows { get; }

        /// <summary>
        ///  Get the width (number of columns) of this grid.
        /// </summary>
        int Cols { get; }

        /// <summary>
        ///  Get a value in the grid.
        /// </summary>
        /// <param name="x">X index of the value.</param>
        /// <param name="y">Y index of the value.</param>
        TValue this[int x, int y] { get; }

        /// <summary>
        ///  Get a value in the grid.
        /// </summary>
        /// <param name="p">Cell index.</param>
        TValue this[Point2 index] { get; }

        /// <summary>
        ///  Get an object reference to synchronize on when using this object from multiple threads.
        /// </summary>
        /// <remarks>
        ///  The same reference will always be returned for the same grid and this will never be the same as the
        ///  reference returned for a different grid (even for a clone).
        /// </remarks>
        object SyncRoot { get; }

        /// <summary>
        ///  Get a modification tracker value.
        /// </summary>
        /// <remarks>
        ///  This value is incremented each time a change is made to the object that would invalidate any active
        ///  enumerators. This allows the enumerator to detect changes and thrown an exception to warn the user.
        /// </remarks>
        int Version { get; }

        /// <summary>
        ///  Copy cells to an array.
        /// </summary>
        /// <param name="array">Array to copy values to.</param>
        /// <param name="startIndex">Array index to start copying values.</param>
        void CopyTo(TValue[] array, int startIndex);

        /// <summary>
        ///  Check if a value is in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparer">Optional equality comparer to use.</param>
        /// <returns>True if the value exists in the grid, false otherwise.</returns>
        bool Contains(TValue value, IEqualityComparer<TValue> comparer);

        /// <summary>
        ///  Enumerate all values in a row.
        /// </summary>
        /// <remarks>
        ///  Values are visited in ascending column order (eg smaller column indices are visted earlier).
        /// </remarks>
        /// <param name="y">Row index.</param>
        /// <returns>Grid cell enumerator.</returns>
        IReadOnlyGridEnumerator<TValue> EnumerateRow(int y);

        /// <summary>
        ///  Enumerate all values in the column.
        /// </summary>
        /// <remarks>
        ///  Values are visited in ascending row order (eg smaller row indices are visted earlier).
        /// </remarks>
        /// <param name="x">Column index.</param>
        /// <returns>Grid cell enumerator.</returns>
        IReadOnlyGridEnumerator<TValue> EnumerateColumn(int x);

        /// <summary>
        ///  Enumerate all values in a rectangular region in the grid.
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
        IReadOnlyGridEnumerator<TValue> EnumerateRectangle(int startX, int startY, int cols, int rows);

        /// <summary>
        ///  Get the index of a value in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <returns>The index of the value in the grid.</returns>
        Point2? IndexOf(TValue value);

        /// <summary>
        ///  Get the index of a value in the grid.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparer">Optional equality comparer to use.</param>
        /// <returns>The index of the value in the grid.</returns>
        Point2? IndexOf(TValue value, IEqualityComparer<TValue> comparer);

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
        TValue Get(int x, int y);
    }

    /// <summary>
    ///  Two dimensional grid.
    /// </summary>
    /// <remarks>
    ///  Cartesian coordinate system with the origin in the top left corner. X increases to the right, and Y increases down.
    /// </remarks>
    public interface IGrid<TValue> : IReadOnlyGrid<TValue>
    {
        /// <summary>
        ///  Get or set a value in the grid.
        /// </summary>
        /// <param name="x">X index of the value.</param>
        /// <param name="y">Y index of the value.</param>
        new TValue this[int x, int y] { get; set; }

        /// <summary>
        ///  Get or set a value in the grid.
        /// </summary>
        /// <param name="p">Cell index.</param>
        new TValue this[Point2 index] { get; set; }

        /// <summary>
        ///  Clear all values in the grid to their default value. The grid size is not changed.
        /// </summary>
        void Clear();

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
        void Set(int x, int y, TValue value);

        /// <summary>
        ///  Set a value in the grid.
        /// </summary>
        /// <remarks>
        ///  This method performs bounds checking, unlike the subscript operator. Calling Set with an invalid position
        ///  will throw an ArgumentOutOfRange exception.
        /// </remarks>
        /// <param name="index">Index of the value in the grid.</param>
        /// <param name="value">Value to put in the cell.</param>
        void Set(Point2 index, TValue value);

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
        void Resize(int newCols, int newRows);
    }
}
