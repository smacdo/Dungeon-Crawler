using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forge.Tilemaps;
using System.Collections.Generic;
using Forge;

namespace Forge.Tests.Tilemaps
{
    [TestClass]
    public class GridPathfinderTests
    {
        [TestMethod]
        public void Pathfind_When_Goal_Is_Start()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            var result = Pathfind_Straight(new Point2(1, 1), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] {}, result);
        }

        [TestMethod]
        public void Pathfind_Straight_When_Goal_Is_One_Cell_Away_In_All_Directions()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // North.
            var result = Pathfind_Straight(new Point2(1, 1), new Point2(1, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0) }, result);

            // East.
            result = Pathfind_Straight(new Point2(1, 1), new Point2(2, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(2, 1) }, result);

            // South.
            result = Pathfind_Straight(new Point2(1, 1), new Point2(1, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 2) }, result);

            // West.
            result = Pathfind_Straight(new Point2(1, 1), new Point2(0, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1) }, result);
        }

        [TestMethod]
        public void Pathfind_From_Top_Left_To_Arbitrary_Cell_Multiple_Cells_Away()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // Path from top left to top right.
            var result = Pathfind_Straight(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(2, 0) }, result);

            // Path from top left to bottom left.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1), new Point2(0, 2) }, result);

            // Path from top left to middle has two solutions but path finder prefers to go east first.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(1, 1) }, result);

            // Path from top left to bottom right has multiple solutions but path finder prefers to go east first.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(1, 0), new Point2(2, 0), new Point2(2, 1), new Point2(2, 2)
            }, result);
        }

        [TestMethod]
        public void Pathfind_From_Top_Left_To_Arbitrary_Cell_Multiple_Cells_Away_With_Cost()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 6, 1 },
                { 1, 3, 1 },
                { 1, 1, 1 }
            });

            // Path from top left to top right.
            var result = Pathfind_Straight(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(1, 1),
                new Point2(2, 1),
                new Point2(2, 0)
            }, result);
            
            // Path from top left to middle.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(1, 1)
            }, result);

            // Path from top left to bottom right.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(0, 2),
                new Point2(1, 2),
                new Point2(2, 2)
            }, result);

            // Path from top left to middle right.
            result = Pathfind_Straight(new Point2(0, 0), new Point2(2, 1), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(1, 1),
                new Point2(2, 1)
            }, result);
        }

        [TestMethod]
        public void Pathfind_Diagonal_When_Goal_Is_One_Cell_Away_In_All_Directions()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // North.
            var result = Pathfind_Diagonal(new Point2(1, 1), new Point2(1, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0) }, result);
            
            // East.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(2, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(2, 1) }, result);

            // South.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(1, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 2) }, result);

            // West.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(0, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1) }, result);

            // North east.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(2, 0) }, result);

            // South east.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(2, 2) }, result);

            // South west.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 2) }, result);

            // North west.
            result = Pathfind_Diagonal(new Point2(1, 1), new Point2(0, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 0) }, result);
        }

        [TestMethod]
        public void Pathfind_Diagonal_From_Top_Left_To_Arbitrary_Cell_Multiple_Cells_Away()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // Path from top left to top right.
            var result = Pathfind_Diagonal(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(2, 0) }, result);

            // Path from top left to bottom left.
            result = Pathfind_Diagonal(new Point2(0, 0), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1), new Point2(0, 2) }, result);

            // Path from top left to middle has two solutions but path finder prefers to go east first.
            result = Pathfind_Diagonal(new Point2(0, 0), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 1) }, result);

            // Path from top left to bottom right has multiple solutions but path finder prefers to go east first.
            result = Pathfind_Diagonal(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 1), new Point2(2, 2) }, result);
        }

        [TestMethod]
        public void Pathfind_Gives_Prev_From_Point_To_Cost_Function()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // Use a custom cost estimator that makes turns really expensive which should minimize the
            // number of corners in a path.

            // Path from top left to top right.
            var result = Pathfind_StraightExpensiveTurns(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(2, 0) }, result);

            // Path from top left to bottom left.
            result = Pathfind_StraightExpensiveTurns(new Point2(0, 0), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1), new Point2(0, 2) }, result);

            // Path from top left to bottom right.
            result = Pathfind_StraightExpensiveTurns(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(0, 2),
                new Point2(1, 2),
                new Point2(2, 2)
            }, result);
        }

        [TestMethod]
        public void Pathfind_Returns_Null_If_No_Path_Found()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 0, 0, 1 },
                { 1, 0, 1 }
            });

            // Path from top right to bottom left.
            var result = Pathfind_DiagonalZeroInfinite(new Point2(2, 0), new Point2(0, 2), grid);
            Assert.IsNull(result);
        }

        private Point2[] Pathfind_Straight(Point2 start, Point2 end, Grid<int> grid)
        {
            var pf = new GridPathfinder<int>(grid, GetMovementCost_Straight, GetManhattanDistance);
            return ToArray(pf.CalculatePath(start, end));
        }

        private Point2[] Pathfind_Diagonal(Point2 start, Point2 end, Grid<int> grid)
        {
            var pf = new GridPathfinder<int>(grid, GetMovementCost_Diagonal, GetDiagonalDistance);
            pf.AllowDiagonalAdjacency = true;

            return ToArray(pf.CalculatePath(start, end));
        }

        private Point2[] Pathfind_DiagonalZeroInfinite(Point2 start, Point2 end, Grid<int> grid)
        {
            var pf = new GridPathfinder<int>(grid, GetMovementCost_Diagonal_ZeroInfinite, GetDiagonalDistance);
            pf.AllowDiagonalAdjacency = true;

            return ToArray(pf.CalculatePath(start, end));
        }

        private Point2[] Pathfind_StraightExpensiveTurns(Point2 start, Point2 end, Grid<int> grid)
        {
            var pf = new GridPathfinder<int>(grid, GetMovementCost_Straight_ExpensiveTurns, GetDiagonalDistance);
            return ToArray(pf.CalculatePath(start, end));
        }

        private T[] ToArray<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return null;
            }

            var result = new List<T>();

            foreach (var v in enumerable)
            {
                result.Add(v);
            }

            return result.ToArray();
        }
        
        private Grid<int> CreateGrid(int rows, int cols, int[,] values)
        {
            var grid = new Grid<int>(cols, rows);

            if (values == null)
            {
                grid.Fill((Grid<int> g, int x, int y) => { return 0; });
            }
            else
            {
                for (int y = 0; y < values.GetLength(0); y++)
                {
                    for (int x = 0; x < values.GetLength(1); x++)
                    {
                        grid[x, y] = values[y, x];
                    }
                }
            }

            return grid;
        }

        private float GetManhattanDistance(Grid<int> grid, Point2 from, Point2 goal)
        {
            const float MovementCost = 1.0f;

            var dx = Math.Abs(from.X - goal.X);
            var dy = Math.Abs(from.Y - goal.Y);

            return MovementCost * (dx + dy);
        }

        private float GetDiagonalDistance(Grid<int> grid, Point2 from, Point2 goal)
        {
            const float StraightMovementCost = 1.0f;
            const float DiagonalMovementCost = 1.41421356237309504880f;  // sqrt(2).

            var dx = Math.Abs(from.X - goal.X);
            var dy = Math.Abs(from.Y - goal.Y);

            return
                StraightMovementCost * (dx + dy) +
                (DiagonalMovementCost - 2.0f * StraightMovementCost) * Math.Min(dx, dy);
        }

        private float GetMovementCost_Straight(Grid<int> grid, Point2 from, Point2 to, Point2 prevFrom)
        {
            Assert.AreEqual(1.0f, Point2.Distance(from, to));
            return Point2.Distance(from, to) * grid[to];
        }

        private float GetMovementCost_Diagonal(Grid<int> grid, Point2 from, Point2 to, Point2 prevFrom)
        {
            //Assert.AreEqual(1.0f, Point2.Distance(from, to));
            return Point2.Distance(from, to) * grid[to];
        }

        private float GetMovementCost_Diagonal_ZeroInfinite(Grid<int> grid, Point2 from, Point2 to, Point2 prevFrom)
        {
            //Assert.AreEqual(1.0f, Point2.Distance(from, to));

            if (grid[to] == 0)
            {
                return float.PositiveInfinity;
            }
            else
            {
                return GetMovementCost_Diagonal(grid, from, to, prevFrom);
            }
        }

        private float GetMovementCost_Straight_ExpensiveTurns(Grid<int> grid, Point2 from, Point2 to, Point2 prevFrom)
        {
            var dx = Math.Abs(prevFrom.X - to.X);
            var dy = Math.Abs(prevFrom.Y - to.Y);

            if (dx != 0.0f && dy != 0.0f)
            {
                return 5 * GetMovementCost_Straight(grid, from, to, prevFrom);
            }
            else
            {
                return GetMovementCost_Straight(grid, from, to, prevFrom);
            }
        }
    }
}
