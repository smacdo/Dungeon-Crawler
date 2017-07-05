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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scott.Forge.Random;
using Scott.Forge.Tilemaps;
using Scott.Forge;

namespace Scott.DungeonCrawler.WorldGeneration
{
    public class RoomLayout
    {
        public RoomLayout(Grid<Tile> tiles, Point2 safeCenter)
        {
            Tiles = tiles;
            SafeCenter = safeCenter;
        }

        public Grid<Tile> Tiles;
        public Point2 SafeCenter;
    }

    public class RoomGenerator
    {
        public TileDefinition FloorTile { get; set; }
        public TileDefinition WallTile { get; set; }
        public int MinWidth { get; set; }
        public int MaxWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }

        public RoomLayout Generate(ForgeRandom random)
        {
            // Validate room generation parameters.
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (MinWidth < 3)
            {
                throw new InvalidOperationException("Min width must be at least three in size");
            }
            else if (MinWidth > MaxWidth)
            {
                throw new InvalidOperationException("Min width must be less than or equal to max width");
            }

            if (MinHeight < 3)
            {
                throw new InvalidOperationException("Min height must be at least three in size");
            }
            else if (MinHeight > MaxHeight)
            {
                throw new InvalidOperationException("Min height must be less than or equal to max height");
            }

            // Generate random room.
            var roomWidth = random.NextInt(MinWidth, MaxWidth + 1);
            var roomHeight = random.NextInt(MinHeight, MaxHeight + 1);

            var tiles = new Grid<Tile>(roomWidth, roomHeight);

            // Fill with floor type.
            tiles.Fill(SetFloor);

            // Carve walls.
            tiles.Fill(x: 0, y: 0, cols: roomWidth, rows: 1, action: SetWall);
            tiles.Fill(x: 0, y: 0, cols: 1, rows: roomHeight, action: SetWall);
            tiles.Fill(x: roomWidth - 1, y: 0, cols: 1, rows: roomHeight, action: SetWall);
            tiles.Fill(x: 0, y: roomHeight - 1, cols: roomWidth, rows: 1, action: SetWall);

            // Pick a safe center location that can be walked on and pathed to.
            // TODO: Add logic that if the picked spot is invalid another spot can be picked.
            var safeCenter = new Point2(tiles.Cols / 2, tiles.Rows / 2);

            return new RoomLayout(tiles, safeCenter);
        }

        private Tile SetFloor(Grid<Tile> grid, int x, int y)
        {
            return new Tile(FloorTile.Id);
        }

        private Tile SetWall(Grid<Tile> grid, int x, int y)
        {
            var t = new Tile(WallTile.Id);
            t.SetImpassable();

            return t;
        }
    }
}
