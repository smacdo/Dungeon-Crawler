/*
 * Copyright 2012-2014 Scott MacDonald
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
using Scott.Forge;
using Scott.Forge.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.DungeonCrawler.WorldGeneration
{
    /// <summary>
    ///  Generates halls between two points in a dungeon according to given parametres.
    /// </summary>
    public class HallGenerator
    {
        private GridPathfinder<Tile> mPathfinder;

        public HallGenerator(Grid<Tile> grid, TileDefinition floor, TileDefinition wall)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            mPathfinder = new GridPathfinder<Tile>(grid, CalculateMovementCost, EstimateRemainingCost);

            Floor = floor;
            Wall = wall;
        }

        public TileDefinition Floor { get; set; }
        public TileDefinition Wall { get; set; }

        public float FloorWalkCost { get; set; } = 1.0f;
        public float WallWalkCost { get; set; } = 8.0f;
        public float DefaultWalkCost { get; set; } = 3.0f;

        public float TurningCost { get; set; } = 10.0f;

        public List<Point2> CreatePath(Point2 from, Point2 to)
        {
            return mPathfinder.CalculatePath(from, to).ToList();
        }

        private float CalculateMovementCost(Grid<Tile> grid, Point2 from, Point2 to, Point2 prevFrom)
        {
            var dx = Math.Abs(prevFrom.X - to.X);
            var dy = Math.Abs(prevFrom.Y - to.Y);

            var tile = grid[to];
            var cost = DefaultWalkCost;

            if (tile.Type == Floor.Id)           // TODO: Query isFloor via tileset.
            {
                cost = FloorWalkCost;
            }
            else if (tile.Type == Wall.Id)      // TODO: Query isWall via tileset.
            {
                cost = WallWalkCost;
            }

            if (dx != 0.0f && dy != 0.0f)
            {
                cost += TurningCost;
            }

            return cost;
        }

        private float EstimateRemainingCost(Grid<Tile> grid, Point2 from, Point2 goal)
        {
            // Manhatten distance.
            const float MovementCost = 1.0f;

            var dx = Math.Abs(from.X - goal.X);
            var dy = Math.Abs(from.Y - goal.Y);

            return MovementCost * (dx + dy);
        }
    }
}
