/*
 * Copyright 2012-2017 Scott MacDonald
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
using Forge;
using Forge.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forge.Spatial;

namespace DungeonCrawler.WorldGeneration
{
    /// <summary>
    ///  Assists in the building of dungeons.
    /// </summary>
    public class DungeonBuilder
    {
        private TileSet mTileSet;
        private TileMap mTileMap;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="cols">Number of columns in the tile grid.</param>
        /// <param name="rows">Number of rows in the tile grid.</param>
        /// <param name="tileSet">Tile set ot use.</param>
        /// <param name="voidTile">Default tile to place.</param>
        public DungeonBuilder(int cols, int rows, TileSet tileSet, TileDefinition voidTile)
        {
            if (tileSet == null)
            {
                throw new ArgumentNullException(nameof(tileSet));
            }

            Grid = new Grid<Tile>(cols, rows);
            mTileSet = tileSet;

            mTileMap = new TileMap(mTileSet, Grid);
            mTileMap.Grid.Fill((IReadOnlyGrid<Tile> g, int x, int y) => { return new Tile(voidTile.Id); });
        }

        /// <summary>
        ///  Get grid used by the dungeon generator.
        /// </summary>
        public Grid<Tile> Grid { get; private set; }

        /// <summary>
        ///  Finalize dungeon building and return a tile map for the dungeon.
        /// </summary>
        /// <returns>The generated dungeon.</returns>
        public TileMap Finalize()
        {
            return mTileMap;
        }

        /// <summary>
        ///  Carve a hallway between two points on the map.
        /// </summary>
        /// <param name="path">List of points along the hallway.</param>
        /// <param name="floor">Tile to use for floors.</param>
        /// <param name="wall">Tile  to use for walls.</param>
        /// <param name="doorway">Tile to use for doors.</param>
        public void CarveHallway(
            List<Point2> path,
            TileDefinition floor,
            TileDefinition wall,
            TileDefinition doorway)
        {
            // TODO: Handle obstacles in a room (not room exterior wall).
            // TODO: Handle cases where hall is impossible to carve.

            foreach (var p in path)
            {
                // Do not dig if the point is in the middle of a room.
                Tile t = Grid[p];

                if (Grid[p].HasFlag((ushort)TileDataFlags.Room))
                {
                    if (mTileSet[Grid[p].Type].IsWall())    // TODO: Only for exterior wall.
                    {
                        // Insert doorway
                        t = new Tile(doorway.Id);
                        t.SetFlag((ushort)TileDataFlags.Placed, true);
                    }
                }
                else
                {
                    // Set tile to be floor.
                    t = new Tile(floor.Id);
                    t.SetFlag((ushort)TileDataFlags.Placed, true);

                    // Add walls in all directions that are empty.
                    TryCarveHallWall(p + Point2.Right, wall);
                    TryCarveHallWall(p + Point2.Down, wall);
                    TryCarveHallWall(p + Point2.Left, wall);
                    TryCarveHallWall(p + Point2.Up, wall);
                }

                Grid[p] = t;
            }
        }

        private void TryCarveHallWall(Point2 at, TileDefinition wall)
        {
            // Do not overwrite existing tiles with a wall.
            if (!Grid[at].HasFlag((ushort)TileDataFlags.Placed))
            {
                var t = new Tile(wall.Id);

                t.SetFlag((ushort)TileDataFlags.Placed, true);
                t.SetImpassable();

                Grid[at] = t;
            }
        }

        /// <summary>
        ///  Carve a room into the tile map.
        /// </summary>
        public bool TryCarveRoom(RoomLayout room, Point2 topLeft)
        {
            // Check if room is overlapped another feature in the map.
            if (DoesOverlap(room.Tiles, topLeft))
            {
                return false;
            }

            // TODO: Copy this code into a Grid helper utility with tests.
            // Copy the room.
            for (int y = 0; y < room.Tiles.Rows; y++)
            {
                for (int x = 0; x < room.Tiles.Cols; x++)
                {
                    var position = new Point2(x + topLeft.X, y + topLeft.Y);
                    var t = room.Tiles[x, y];

                    t.SetFlag((ushort)TileDataFlags.Placed, true);
                    t.SetFlag((ushort)TileDataFlags.Room, true);

                    Grid[position] = t;
                }
            }

            return true;
        }

        private bool DoesOverlap(Grid<Tile> roomTiles, Point2 topLeft)
        {
            for (int y = 0; y < roomTiles.Rows; y++)
            {
                for (int x = 0; x < roomTiles.Cols; x++)
                {
                    var dungeonTile = Grid[x + topLeft.X, y + topLeft.Y];
                    var dungeonTileDef = mTileSet[dungeonTile.Type];

                    var roomTile = roomTiles[x, y];
                    var roomTileDef = mTileSet[roomTile.Type];

                    if (!dungeonTileDef.IsVoid() && !(dungeonTileDef.IsWall() && roomTileDef.IsWall()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
