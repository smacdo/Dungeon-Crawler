/*
 * Copyright 2017 Scott MacDonald
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
using Scott.Forge.Support;
using System;
using System.Collections.Generic;

namespace Scott.Forge.Tilemaps
{
    public delegate float PathCostDelegate<CellType>(Grid<CellType> grid, Point2 from, Point2 to);

    /// <summary>
    ///  A customizable 2d grid based path finder based on A-Star.
    ///  TODO: Needs a lot of optimization, right now allocates a lot of stuff.
    /// </summary>
    public class GridPathfinder<CellType>
    {
        private PriorityQueue<Point2, float> mFrontier;
        private Grid<PathCell> mCells;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="grid">Grid to path find over.</param>
        /// <param name="actualCostDelegate">TODO</param>
        /// <param name="estimatedCostDelegate">TODO</param>
        public GridPathfinder(
                Grid<CellType> grid,
                PathCostDelegate<CellType> actualCostDelegate,
                PathCostDelegate<CellType> estimatedCostDelegate)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (actualCostDelegate == null)
            {
                throw new ArgumentNullException(nameof(actualCostDelegate));
            }

            if (estimatedCostDelegate == null)
            {
                throw new ArgumentNullException(nameof(estimatedCostDelegate));
            }

            Grid = grid;
            ActualCostDelegate = actualCostDelegate;
            EstimatedCostDelegate = estimatedCostDelegate;

            mFrontier = new PriorityQueue<Point2, float>(EstimateMaxOpenNodeCount(Grid));
            mCells = new Grid<PathCell>(Grid.Cols, Grid.Rows);
        }

        public Grid<CellType> Grid { get; private set; }

        public PathCostDelegate<CellType> ActualCostDelegate { get; set; }

        public PathCostDelegate<CellType> EstimatedCostDelegate { get; set; }

        public bool AllowDiagonalAdjacency { get; set; }

        public IEnumerable<Point2> CalculatePath(Point2 start, Point2 goal)
        {
            mFrontier.Clear();
            mCells.Clear();

            mFrontier.Add(start, 0);
            mCells[start].MakeOpen();

            bool didFindPath = false;

            while (!mFrontier.IsEmpty)
            {
                var currentPosition = mFrontier.Remove();
                
                // Is this the goal?
                if (currentPosition == goal)
                {
                    didFindPath = true;
                    break;
                }

                // Visit straight edge neighbors.
                VisitNeighbors(currentPosition);

                // Set current cell to be closed now that all neighbors have been inspected.
                mCells[currentPosition].MakeClosed();
            }

            // TODO: Handle where path could not be generated.

            // Calculate a path from the start to the goal and return it.
            return didFindPath ? GetPathToStart(start, goal) : null;
        }

        private List<Point2> GetPathToStart(Point2 start, Point2 goal)
        {
            var selectedPath = new List<Point2>();      // TOD: Presize and maybe use a pool.
            //selectedPath.Add(goal);

            var currentPosition = goal;

            while (currentPosition != start)
            {
                selectedPath.Add(currentPosition);
                currentPosition = mCells[currentPosition].CameFrom;
            }

            // TODO: Add test for infinite following (when grid is broken).

            //selectedPath.Add(start);    // TODO: Optional, not sure if best.
            selectedPath.Reverse();

            return selectedPath;
        }

        private void VisitNeighbors(Point2 position)
        {
            // East neighbor.
            if (position.X < Grid.Cols - 1)
            {
                VisitNeighbor(position, new Point2(1, 0));
            }

            // South.
            if (position.Y < Grid.Rows - 1)
            {
                VisitNeighbor(position, new Point2(0, 1));
            }

            // West.
            if (position.X > 0)
            {
                VisitNeighbor(position, new Point2(-1, 0));
            }

            // North.
            if (position.Y > 0)
            {
                VisitNeighbor(position, new Point2(0, -1));
            }
        }

        private void VisitNeighbor(Point2 position, Point2 offset)
        {
            var neighborPosition = position + offset;

            // Get the cost of moving from the start to this tile (the actual cost).
            var moveCost = mCells[position].BestCost;
            moveCost += ActualCostDelegate(Grid, position, neighborPosition);

            // Check if this node is currently open with a larger cost. If true then replace the entry with this entry
            // in the open set.

            // ...
            var cell = mCells[neighborPosition];

            if (moveCost < cell.BestCost)
            {
                if (cell.IsOpen)
                {
                    cell.State = CellState.Default;
                    mFrontier.Remove(neighborPosition);
                }
                else if (cell.IsClosed)
                {
                    // This should not happen with a consistent admissible heuristic.
                    // TODO: Remove neighbor from closed.
                    cell.State = CellState.Default;
                }
            }
            
            // ...
            if (cell.IsClosed == false && cell.IsOpen == false)
            {
                cell.BestCost = moveCost;
                cell.MakeOpen();

                var estimatedCost = EstimatedCostDelegate(Grid, position, neighborPosition);
                var newScore = moveCost + estimatedCost;

                mFrontier.Add(neighborPosition, newScore);

                cell.CameFrom = position;
            }

            // ...
            mCells[neighborPosition] = cell;
        }

        private int EstimateMaxOpenNodeCount(Grid<CellType> grid)
        {
            // TODO: Be smarter about this.
            return grid.Count / 4;
        }

        private int EstimateMaxClosedNodeCount(Grid<CellType> grid)
        {
            // TODO: Be smarter about this.
            return grid.Count / 4;
        }

        private enum CellState
        {
            Default,
            Open,
            Closed
        }

        private struct PathCell
        {
            public CellState State;
            public Point2 CameFrom;
            public float BestCost;

            public void MakeOpen()
            {
                State = CellState.Open;
            }

            public void MakeClosed()
            {
                State = CellState.Closed;
            }
            
            public bool IsOpen
            {
                get { return State == CellState.Open; }
            }

            public bool IsClosed
            {
                get { return State == CellState.Closed; }
            }
        }
        
    }
}
