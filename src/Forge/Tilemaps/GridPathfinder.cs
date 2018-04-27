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
using Forge.Support;
using System;
using System.Collections.Generic;
using Forge.Spatial;

namespace Forge.Tilemaps
{
    public delegate float ActualCostDelegate<CellType>(Grid<CellType> grid, Point2 from, Point2 to, Point2 prevFrom);
    public delegate float EstimatedCostDelegate<CellType>(Grid<CellType> grid, Point2 position, Point2 goal);

    /// <summary>
    ///  A customizable 2d grid based path finder based on A-Star.
    /// </summary>
    public class GridPathfinder<CellType>
    {
        private PriorityQueue<Point2, float> mFrontier;
        private Grid<PathCell> mCells;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="grid">Grid to use for path finding.</param>
        /// <param name="actualCostDelegate">Actual cost delegate from going to one cell to another.</param>
        /// <param name="estimatedCostDelegate">Estimated cost delegate from a cell to the goal.</param>
        public GridPathfinder(
                Grid<CellType> grid,
                ActualCostDelegate<CellType> actualCostDelegate,
                EstimatedCostDelegate<CellType> estimatedCostDelegate)
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
            ActualCoster = actualCostDelegate;
            EstimateCoster = estimatedCostDelegate;

            mFrontier = new PriorityQueue<Point2, float>(EstimateMaxOpenNodeCount(Grid));
            mCells = new Grid<PathCell>(Grid.Cols, Grid.Rows);
        }

        /// <summary>
        ///  Get or set the grid used for path finding.
        /// </summary>
        public Grid<CellType> Grid { get; private set; }

        /// <summary>
        ///  Get or set the cost delegate used to calculate exact cost from one cell to an adjacent cell.
        /// </summary>
        public ActualCostDelegate<CellType> ActualCoster { get; set; }

        /// <summary>
        ///  Get or set the cost delegate used to estimate the cost from a cell to the goal.
        /// </summary>
        public EstimatedCostDelegate<CellType> EstimateCoster { get; set; }

        /// <summary>
        ///  Get or set if diagonal neighbors should be used when path finding.
        /// </summary>
        public bool AllowDiagonalAdjacency { get; set; }

        /// <summary>
        ///  Find a path from the start point to the goal point.
        /// </summary>
        /// <remarks>
        ///  The returned list of points is a list of sequential points to follow to get to the goal and includes the
        ///  goal point but excludes the starting point. If the goal could not be reached, a null list is returned.
        ///  If the start and the goal are on the same square then an empty list is returned.
        /// </remarks>
        /// <param name="start">Point to start at.</param>
        /// <param name="goal">Point to path find to.</param>
        /// <returns>A list of points to follow to reach the goal, or null if no path was found.</returns>
        public IEnumerable<Point2> CalculatePath(Point2 start, Point2 goal)
        {
            mFrontier.Clear();
            mCells.Clear();

            mFrontier.Add(start, 0);
            mCells[start].MakeOpen(0);

            bool didFindPath = false;

            // Continually examine candidate cells until the goal is found or all possible cells have been searched.
            while (!mFrontier.IsEmpty)
            {
                // Get the lowest cost (best) of the remaining cells to examine.
                var currentPosition = mFrontier.Remove();
                
                // Is this the goal?
                if (currentPosition == goal)
                {
                    didFindPath = true;
                    break;
                }

                // Visit and enqueue eligible neighbor cells before closing this cell.
                VisitNeighbors(currentPosition, goal, AllowDiagonalAdjacency);
                mCells[currentPosition].MakeClosed();
            }

            // Calculate a path from the start to the goal and return it.
            return didFindPath ? GetPathToStart(start, goal) : null;
        }

        /// <summary>
        ///  Visit each neighbor of a cell for the A* path finding algorithm.
        /// </summary>
        /// <param name="position">Position of the cell.</param>
        /// <param name="goal">Goal position for the path finding operation.</param>
        /// <param name="allowDiagonal">Should diagonal neighbors be visited.</param>
        private void VisitNeighbors(Point2 position, Point2 goal, bool allowDiagonal)
        {
            VisitStraightNeighbors(position, goal);

            if (allowDiagonal)
            {
                VisitDiagonalNeighbors(position, goal);
            }
        }

        /// <summary>
        ///  Visit the east, south, west and north cell neighbors for the A* path finding algorithm.
        /// </summary>
        /// <param name="position">Position of the cell.</param>
        /// <param name="goal">Goal position for the path finding operation.</param>
        private void VisitStraightNeighbors(Point2 position, Point2 goal)
        {
            // Visit the east neighbor.
            if (position.X < Grid.Cols - 1)
            {
                VisitNeighbor(position, new Point2(1, 0), goal);
            }

            // South.
            if (position.Y < Grid.Rows - 1)
            {
                VisitNeighbor(position, new Point2(0, 1), goal);
            }

            // West.
            if (position.X > 0)
            {
                VisitNeighbor(position, new Point2(-1, 0), goal);
            }

            // North.
            if (position.Y > 0)
            {
                VisitNeighbor(position, new Point2(0, -1), goal);
            }
        }

        /// <summary>
        ///  Visit the diagonal cell neighbors for the A* path finding algorithm.
        /// </summary>
        /// <param name="position">Position of the cell.</param>
        /// <param name="goal">Goal position for the path finding operation.</param>
        private void VisitDiagonalNeighbors(Point2 position, Point2 goal)
        {
            // Visit the south east neighbor.
            if (position.X < Grid.Cols - 1 && position.Y < Grid.Rows - 1)
            {
                VisitNeighbor(position, new Point2(1, 1), goal);
            }

            // South west.
            if (position.X > 0 && position.Y < Grid.Rows - 1)
            {
                VisitNeighbor(position, new Point2(-1, 1), goal);
            }

            // North west.
            if (position.X > 0 && position.Y > 0)
            {
                VisitNeighbor(position, new Point2(-1, -1), goal);
            }

            // North east.
            if (position.X < Grid.Cols - 1 && position.Y > 0)
            {
                VisitNeighbor(position, new Point2(1, -1), goal);
            }
        }

        /// <summary>
        ///  Visit a cell neighbor for a single step of the A* path finding algorithm.
        /// </summary>
        /// <param name="position">Position of the cell.</param>
        /// <param name="offset">Neighbor offset.</param>
        /// <param name="goal">Path finding goal point.</param>
        private void VisitNeighbor(Point2 position, Point2 offset, Point2 goal)
        {
            var neighborPosition = position + offset;

            // Get the cost of moving from the start to this tile (the actual cost).
            var moveCost = mCells[position].MoveCost;
            moveCost += ActualCoster(Grid, position, neighborPosition, mCells[position].CameFrom);

            // Skip this neighbor if the cost is infinite (signals that the cell is not enterable).
            if (float.IsInfinity(moveCost))
            {
                return;
            }

            // Check if this node is currently open with a larger cost. If true then replace the entry with this entry
            // in the open set.

            // Check if the cell is either open or has been closed, and if true then check if the new movement (actual)
            // cost is lower than the old cost.
            var cell = mCells[neighborPosition];

            if (moveCost < cell.MoveCost)
            {
                if (cell.IsOpen)
                {
                    // The cell was is in the list of candidates but this new path has a lower cost to reach this cell.
                    // Remove it from the frontier and reset its search state.
                    cell.State = CellState.Default;
                    mFrontier.Remove(neighborPosition);
                }
                else if (cell.IsClosed)
                {
                    // The cell was already expanded (and should no longer be considered), however this path has a
                    // lower cost to reach this cell. This should never unless the cost estimator delegate generates
                    // costs that are higher than the actual cost. 
                    cell.State = CellState.Default;
                }
            }
            
            // If the neighbor cell has not been expanded nor is a candidate for expansion then add it to the frontier.
            if (cell.IsClosed == false && cell.IsOpen == false)
            {
                // Update the movement (actual) cost of the cell and set its search status to be open.
                cell.MakeOpen(moveCost);

                // Estimate the cost remaining to path from this cell to the goal.
                var estimatedCost = EstimateCoster(Grid, neighborPosition, goal);
                var newScore = moveCost + estimatedCost;

                // Add the cell to the frontier along with its new score.
                mFrontier.Add(neighborPosition, newScore);

                // Make a note of the previous cell so a path can be generated once the goal is reached.
                cell.CameFrom = position;
            }

            // Update the neighbor cell with its new state.
            mCells[neighborPosition] = cell;
        }

        /// <summary>
        ///  Generate a list of points to follow when going from the start point to the goal point.
        /// </summary>
        /// <remarks>
        ///  This method will behave incorrectly (may loop indefinitely) if the start and goal points are not along the
        ///  explored path from the path finder. Additionally start point must occur earlier in the path than goal
        ///  point.
        /// </remarks>
        /// <param name="start">Point that path finding began at.</param>
        /// <param name="goal">Point where path finding terminated at.</param>
        /// <returns></returns>
        private List<Point2> GetPathToStart(Point2 start, Point2 goal)
        {
            // Walk the path and count the number of entries that are encountered.
            // TODO: This could be optimized by having this first iteration reverse the path in addition to counting
            //       the number of cells encountered along the path. Doing this would allow the second iteration to
            //       add the cells from start to goal and avoid the final call to Reverse().
            int cellsInPath = 0;
            var currentPosition = goal;

            while (currentPosition != start)
            {
                cellsInPath += 1;
                currentPosition = mCells[currentPosition].CameFrom;
            }

            // Follow path again but this time add each point to a list that was presized.
            var selectedPath = new List<Point2>(cellsInPath);
            currentPosition = goal;

            while (currentPosition != start)
            {
                selectedPath.Add(currentPosition);
                currentPosition = mCells[currentPosition].CameFrom;
            }

            selectedPath.Reverse();
            return selectedPath;
        }

        /// <summary>
        ///  Estimate the number of open nodes that will be required for the path finding operation.
        /// </summary>
        /// <param name="grid">Grid that will be used for path finding.</param>
        /// <returns>Estimated capacity for path finding.</returns>
        private int EstimateMaxOpenNodeCount(Grid<CellType> grid)
        {
            return grid.Cols * 2 + grid.Rows * 2;
        }

        private enum CellState
        {
            Default,
            Open,
            Closed
        }

        // TODO: Pack the state and came from values into a single byte to reduce memory use.
        private struct PathCell
        {
            // A* cell state (Not visited, open for expansion, closed after expansion).
            public CellState State;

            // Stores the point prior to this point from the A* path finding algorithm. This allows the path finder to
            // reconstruct a path from the goal point to the start point.
            public Point2 CameFrom;
            
            // The move cost is the A* g (actual) cost, which is the cost to move from the start point to this cell.
            public float MoveCost;

            public void MakeOpen(float newCost)
            {
                State = CellState.Open;
                MoveCost = newCost;
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
