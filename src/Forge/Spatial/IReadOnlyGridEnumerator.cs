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

namespace Forge.Spatial
{
    /// <summary>
    ///  Grid enumerator.
    /// </summary>
    /// <remarks>
    ///  TODO: Consider converting this is struct iterator for memory performance. First check that copying multiple
    //         values is still performant and if not then write down in remarks.
    /// </remarks>
    public sealed class IReadOnlyGridEnumerator<TValue> : IEnumerator<TValue>
    {
        private IReadOnlyGrid<TValue> mGrid;

        private int mX = 0;
        private int mY = 0;
        private int mStartX = 0;
        private int mEndX = 0;          // Range: [mStartX, mEndX)
        private int mEndY = 0;          // Range: [startY, mEndY)
        private int mVersion = 0;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="grid">Grid to enumerate.</param>
        public IReadOnlyGridEnumerator(IReadOnlyGrid<TValue> grid)
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
        public IReadOnlyGridEnumerator(IReadOnlyGrid<TValue> grid, int x, int y, int cols, int rows)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            IGridHelpers.ValidateIteratableRegionArguments(x, y, cols, rows, grid.Cols, grid.Rows);

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
                throw new InvalidOperationException("Grid was modified; enumeration operation is invalidated");
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
            throw new NotSupportedException();
        }
    }
}
