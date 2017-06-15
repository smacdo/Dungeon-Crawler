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
    ///  Assists in the building of dungeons.
    /// </summary>
    public class DungeonBuilder
    {
        private Grid<Tile> mGrid;
        private TileSet mTileSet;
        private TileMap mTileMap;

        public DungeonBuilder(int cols, int rows, TileSet tileSet, TileDefinition voidTile)
        {
            if (tileSet == null)
            {
                throw new ArgumentNullException(nameof(tileSet));
            }

            mGrid = new Grid<Tile>(cols, rows);
            mTileSet = tileSet;

            mTileMap = new TileMap(mTileSet, mGrid);
            mTileMap.Grid.Fill((Grid<Tile> g, int x, int y) => { return new Tile(voidTile.Id); });
        }

        public TileMap Build()
        {
            return mTileMap;
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

                    mGrid[position] = room.Tiles[x, y];
                    mGrid[position].SetIsVoid(false);
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
                    var dungeonTile = mGrid[x + topLeft.X, y + topLeft.Y];
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
