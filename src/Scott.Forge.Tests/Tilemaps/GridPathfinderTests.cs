using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scott.Forge.Tilemaps;
using System.Collections.Generic;

namespace Scott.Forge.Tests.Tilemaps
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

            var result = Pathfind_ManhattanDistance(new Point2(1, 1), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] {}, result);
        }

        [TestMethod]
        public void Pathfind_When_Goal_Is_One_Cell_Away_In_All_Directions()
        {
            var grid = CreateGrid(3, 3, new int[,] {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            });

            // North.
            var result = Pathfind_ManhattanDistance(new Point2(1, 1), new Point2(1, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0) }, result);

            // East.
            result = Pathfind_ManhattanDistance(new Point2(1, 1), new Point2(2, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(2, 1) }, result);

            // South.
            result = Pathfind_ManhattanDistance(new Point2(1, 1), new Point2(1, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 2) }, result);

            // West.
            result = Pathfind_ManhattanDistance(new Point2(1, 1), new Point2(0, 1), grid);
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

            var result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(2, 0) }, result);

            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1), new Point2(0, 2) }, result);

            // Two solutions but path finder prefers to go east first.
            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(1, 1) }, result);

            // Two solutions but path finder prefers to go east first.
            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(1, 0), new Point2(1, 1), new Point2(2, 1), new Point2(2, 2)
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

            var result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(2, 0), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(0, 1),
                new Point2(1, 1),
                new Point2(2, 1),
                new Point2(2, 0)
            }, result);

            /*
            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(0, 2), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(0, 1), new Point2(0, 2) }, result);

            // Two solutions but path finder prefers to go east first.
            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(1, 1), grid);
            CollectionAssert.AreEqual(new Point2[] { new Point2(1, 0), new Point2(1, 1) }, result);

            // Two solutions but path finder prefers to go east first.
            result = Pathfind_ManhattanDistance(new Point2(0, 0), new Point2(2, 2), grid);
            CollectionAssert.AreEqual(new Point2[]
            {
                new Point2(1, 0), new Point2(1, 1), new Point2(2, 1), new Point2(2, 2)
            }, result);*/
        }

        // TODO: Test diagonal.
        // TODO: Test where solution could not be found.

        private Point2[] Pathfind_ManhattanDistance(Point2 start, Point2 end, Grid<int> grid)
        {
            var pf = new GridPathfinder<int>(grid, GetExactMovementCost, GetManhattanDistance);
            return ToArray(pf.CalculatePath(start, end));
        }

        private T[] ToArray<T>(IEnumerable<T> enumerable)
        {
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

        private float GetExactMovementCost(Grid<int> grid, Point2 from, Point2 to)
        {
            // TODO: Assert from -> to is only one tile away.
            // TODO: Assert from -> matches allowed movement (straight or diagonals).
            return Point2.Distance(from, to) * grid.Cells[to.X, to.Y];
        }
    }
}
