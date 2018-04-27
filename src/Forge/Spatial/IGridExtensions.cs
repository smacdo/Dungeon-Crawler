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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Forge.Spatial
{
    /// <summary>
    ///  Collection of extension methods for IGrid and IReadOnlyGrid.
    /// </summary>
    public static class IGridExtensions
    {
        /// <summary>
        ///  Invoke the provided action on each cell in the grid.
        /// </summary>
        /// <remarks>
        ///  This method will iterate over every column in row order, so the first row's columns will be iterated over
        ///  and then the second and so on.
        ///  
        ///  The action's first two parameters are the X and Y position of the current cell being visited. The third
        ///  parameter is the value of the cell.
        /// </remarks>
        /// <param name="action">Action to execute.</param>
        public static void ForEach<TValue>(
            this IReadOnlyGrid<TValue> self,
            Action<TValue, int, int> action)
        {
            Debug.Assert(self != null);
            ForEach(self, 0, 0, self.Cols, self.Rows, action);
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
        public static void ForEach<TValue>(
            this IReadOnlyGrid<TValue> self,
            int x,
            int y,
            Action<TValue, int, int> action)
        {
            Debug.Assert(self != null);
            ForEach(self, x, y, -1, -1, action);
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
        public static void ForEach<TValue>(
            this IReadOnlyGrid<TValue> self,
            int x,
            int y,
            int cols,
            int rows,
            Action<TValue, int, int> action)
        {
            Debug.Assert(self != null);

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Check if caller provided a width and height argument. If the caller used the default -1, then calculate
            // the width and height required to go from the startXY positions to the ends of the grid.
            if (cols == -1)
            {
                cols = self.Cols - x;
            }

            if (rows == -1)
            {
                rows = self.Rows - y;
            }

            // Validate arguments.
            IGridHelpers.ValidateIteratableRegionArguments(x, y, cols, rows, self.Cols, self.Rows);

            // Calculate last x and y tiles to visit by using the width/height arguments. 
            var endX = x + cols;
            var endY = y + rows;

            Debug.Assert(endX >= x && endX <= self.Cols);
            Debug.Assert(endY >= y && endY <= self.Rows);

            // Iterate each element.
            var cachedVersion = self.Version;

            for (var yIndex = y; yIndex < endY; yIndex++)
            {
                for (var xIndex = x; xIndex < endX; xIndex++)
                {
                    if (cachedVersion != self.Version)
                    {
                        throw new InvalidOperationException("Grid was modified; enumeration operation is invalidated");
                    }

                    action(self[xIndex, yIndex], xIndex, yIndex);
                }
            }
        }

        /// <summary>
        ///  Set the value of each cell in the grid to the result of calling action on each grid cell.
        /// </summary>
        /// <remarks>
        ///  This method visits each cell in the same order as for each. The first parameter to action is this grid.
        ///  The second and third parameters are the X and Y position of the current cell.
        /// </remarks>
        public static void Fill<TValue>(
            this IGrid<TValue> self,
            Func<IReadOnlyGrid<TValue>, int, int, TValue> action)
        {
            Fill(self, 0, 0, -1, -1, action);
        }

        /// <summary>
        ///  Set the value of each cell in a rectangular region in the grid to the result of calling the action on each
        ///  grid cell.
        /// </summary>
        /// <remarks>
        ///  This method visits each cell in the same order as for each. The first parameter to action is this grid.
        ///  The second and third parameters are the X and Y position of the current cell.
        /// </remarks>
        /// <param name="x">Cell column to start at.</param>
        /// <param name="y">Cell row to start at.</param>
        /// <param name="cols">The number of columns (width) to visit.</param>
        /// <param name="rows">The number of rows (height) to visit.</param>
        /// <param name="action">Function to call for each visited cell.</param>
        public static void Fill<TValue>(
            this IGrid<TValue> self,
            int x,
            int y,
            int cols,
            int rows,
            Func<IReadOnlyGrid<TValue>, int, int, TValue> action)
        {
            Debug.Assert(self != null);

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Check if caller provided a width and height argument. If the caller used the default -1, then calculate
            // the width and height required to go from the startXY positions to the ends of the grid.
            if (cols == -1)
            {
                cols = self.Cols - x;
            }

            if (rows == -1)
            {
                rows = self.Rows - y;
            }

            // Validate arguments.
            IGridHelpers.ValidateIteratableRegionArguments(x, y, cols, rows, self.Cols, self.Rows);

            // Calculate last x and y tiles to visit by using the width/height arguments. 
            var endX = x + cols;
            var endY = y + rows;

            Debug.Assert(endX >= x && endX <= self.Cols);
            Debug.Assert(endY >= y && endY <= self.Rows);

            // Iterate each element.
            var cachedVersion = self.Version;

            for (var yIndex = y; yIndex < endY; yIndex++)
            {
                for (var xIndex = x; xIndex < endX; xIndex++)
                {
                    if (cachedVersion != self.Version)
                    {
                        throw new InvalidOperationException("Grid was modified; enumeration operation is invalidated");
                    }

                    self[xIndex, yIndex] = action(self, xIndex, yIndex);
                }
            }
        }

        /// <summary>
        ///  Extract a portion of the grid and return it as a new grid.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static Grid<TValue> Extract<TValue>(
            this Grid<TValue> grid,
            int startX,
            int startY,
            int cols,
            int rows)
        {
            // Cannot extract from a null grid.
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            // TODO: This was written quickly and needs te finished/polished.

            // Check if caller provided a width and height argument. If the caller used the default -1, then calculate
            // the width and height required to go from the startXY positions to the ends of the grid.
            if (cols == -1)
            {
                cols = grid.Cols - startX;
            }

            if (rows == -1)
            {
                rows = grid.Rows - startY;
            }

            // Calculate last x and y tiles to visit by using the width/height arguments. 
            int lastX = startX + cols - 1;
            int lastY = startY + rows - 1;

            // Check boundaries before enumeration.
            if (startX < 0 || startX >= grid.Cols)
            {
                throw new ArgumentException("Out of bounds", nameof(startX));
            }

            if (startY < 0 || startY >= grid.Rows)
            {
                throw new ArgumentException("Out of bounds", nameof(startY));
            }

            if (lastX >= grid.Cols || lastX < startX)
            {
                throw new ArgumentException("Out of bounds", nameof(cols));
            }

            if (lastY >= grid.Rows || lastY < startY)
            {
                throw new ArgumentException("Out of bounds", nameof(rows));
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationStartX"></param>
        /// <param name="destinationStartY"></param>
        public static void Insert<TValue>(
            this Grid<TValue> destination,
            Grid<TValue> source,
            int destinationStartX,
            int destinationStartY)
        {
            Insert(destination, source, destinationStartX, destinationStartY, 0, 0, -1, -1);
        }

        public static void Insert<TValue>(
            this Grid<TValue> destination,
            Grid<TValue> source,
            int destinationStartX,
            int destinationStartY,
            int sourceStartX,
            int sourceStartY)
        {
            Insert(destination, source, destinationStartX, destinationStartY, sourceStartX, sourceStartY, -1, -1);
        }

        /// <summary>
        ///  Insert a grid into this grid at the specified point by copying values. If this grid is not large enough to
        ///  accomodate the grid, it will not be resized.
        /// </summary>
        /// <param name="source">Another grid to copy values from.</param>
        /// <param name="destinationStartX"></param>
        /// <param name="destinationStartY"></param>
        /// <param name="sourceStartX"></param>
        /// <param name="sourceStartY"></param>
        /// <param name="rowCopyCount"></param>
        /// <param name="colCopyCount"></param>
        public static void Insert<TValue>(
            this Grid<TValue> destination,
            Grid<TValue> source,
            int destinationStartX,
            int destinationStartY,
            int sourceStartX,
            int sourceStartY,
            int colCopyCount,
            int rowCopyCount)
        {
            // Cannot copy to a null grid or from a null grid.
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            // Check for invalid destination indices.
            if (destinationStartX < 0 || destinationStartX > destination.Cols)
            {
                throw new ArgumentException("Destination start x index is out of bounds", nameof(destinationStartX));
            }

            if (destinationStartY < 0 || destinationStartY > destination.Rows)
            {
                throw new ArgumentException("Destination start y index is out of bounds", nameof(destinationStartY));
            }

            // Validate that the region we want to copy from (in the other grid) is not out of bounds.
            if (sourceStartX < 0 || sourceStartX >= source.Cols)
            {
                throw new ArgumentException("Source start x index is out of bounds", nameof(sourceStartX));
            }

            if (sourceStartY < 0 || sourceStartY >= source.Rows)
            {
                throw new ArgumentException("Source start y index is out of bounds", nameof(sourceStartY));
            }

            // Check if the caller provided the requested number of columns and rows to copy from the requested grid.
            // If not, assume they want to copy all the columns and rows. 
            if (colCopyCount == -1)
            {
                colCopyCount = source.Cols - sourceStartX;
            }
            else if (colCopyCount < 1)
            {
                throw new ArgumentNullException(nameof(colCopyCount), "Source column count must be larger than zero");
            }

            if (rowCopyCount == -1)
            {
                rowCopyCount = source.Rows - sourceStartY;
            }
            else if (rowCopyCount < 1)
            {
                throw new ArgumentNullException(nameof(rowCopyCount), "Source row count must be larger than zero");
            }

            // Check that source column copy count does not result in an out of bounds situation.
            if (sourceStartX + colCopyCount >= source.Cols)
            {
                throw new ArgumentException("Source column copy count too large", nameof(colCopyCount));
            }

            if (sourceStartY + rowCopyCount >= source.Rows)
            {
                throw new ArgumentException("Source row copy count too large", nameof(rowCopyCount));
            }

            // Check that the destination region is in bounds.
            if (destinationStartX + colCopyCount >= destination.Cols)
            {
                throw new InvalidOperationException("Grid is not wide enough");
            }

            if (destinationStartY + rowCopyCount >= destination.Rows)
            {
                throw new InvalidOperationException("Grid is not tall enough");
            }

            // Compute the source and destination rectangle boundaries to copy.
            var sourceLastX = destinationStartX + colCopyCount;
            var sourceLastY = destinationStartY + rowCopyCount;
            var destLastX = sourceStartX + colCopyCount;
            var destLastY = sourceStartY + rowCopyCount;

            Debug.Assert(sourceLastX <= destination.Cols);
            Debug.Assert(sourceLastY <= destination.Rows);
            Debug.Assert(destLastX <= source.Cols);
            Debug.Assert(destLastY <= source.Rows);

            // Start copying tiles.
            for (int y = 0; y < (destLastY - sourceStartY); y++)
            {
                for (int x = 0; x < (destLastX - sourceStartX); x++)
                {
                    destination.Cells[y + destinationStartY, x + destinationStartX] =
                        source.Cells[y + sourceStartY, x + sourceStartX];
                }
            }
        }
    }

    /// <summary>
    ///  Utility methods for IGrid classes.
    /// </summary>
    internal static class IGridHelpers
    {
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
}
