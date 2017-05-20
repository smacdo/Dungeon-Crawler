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
using Scott.Forge;
using Scott.Forge.Random;
using Scott.Forge.Tilemaps;

namespace Scott.DungeonCrawler.WorldGeneration
{
    /// <summary>
    ///  Generates a dungeon for the player to explore.
    /// </summary>
    public class DungeonGenerator
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="tileset">Tile set to use for the map.</param>
        public DungeonGenerator(TileSet tileset)
        {
            if (tileset == null)
            {
                throw new ArgumentNullException(nameof(tileset));
            }

            TileSet = tileset;
            Random = new ForgeRandom();
        }

        /// <summary>
        ///  Get or set the tile set used for this dungeon.
        /// </summary>
        public TileSet TileSet { get; private set; }
        
        /// <summary>
        ///  Get or set the tile id for empty tiles.
        /// </summary>
        public ushort EmptyTileId { get; set; } = 0;

        /// <summary>
        ///  Get or set the tile id for walls.
        /// </summary>
        public ushort WallTileId { get; set; } = 0;

        /// <summary>
        ///  Get or set the tile id for floors.
        /// </summary>
        public ushort FloorTileId { get; set; } = 0;

        /// <summary>
        ///  Get or set the random number generator.
        /// </summary>
        public ForgeRandom Random { get; set; }

        /// <summary>
        ///  Get the list of room generators to use when randomly generating rooms.
        /// </summary>
        public List<RoomGenerator> RoomGenerators { get; private set; } = new List<RoomGenerator>();

        public int MaxRoomCount { get; private set; } = 150;
        public int MaxRoomRetryCount { get; private set; } = 5;

        /// <summary>
        ///  Generate a new random dungeon.
        /// </summary>
        /// <param name="cols">Maximum number of columns in the tile map.</param>
        /// <param name="rows">Maximum number of rows in the tile map.</param>
        /// <returns>A new tile map.</returns>
        public TileMap Generate(int cols, int rows)
        {
            var grid = new Grid<Tile>(cols, rows);

            // Create tilemap and ppopulate it with stuff.
            var tilemap = new TileMap(TileSet, cols, rows);
            tilemap.Grid.Fill((Grid<Tile> g, int x, int y) => { return new Tile(EmptyTileId); });

            // Create 50 new rooms in random locations.
            var roomCount = MaxRoomCount;
            var rooms = new List<RoomLayout>(roomCount);

            for (int i = 0; i < roomCount; i++)
            {
                // Pick a random room generator from the list.
                // TODO: Add weights.
                var selectedIndex = Random.NextInt(0, RoomGenerators.Count);
                var roomGenerator = RoomGenerators[selectedIndex];

                // Generate a room and add it to the set of rooms.
                rooms.Add(roomGenerator.Generate(Random));
            }
            
            // Iterate through the list of rooms and place them into the world at a random location. Try up to X
            // number of times before moving onto the next room.
            // TODO: Use separation algorithm rather than randomly trying spots.
            // TODO: Add collision detection to make sure rooms don't overwrite each other.
            // TODO: Add a retry.
            for (int i = 0; i < rooms.Count; i++)
            {
                bool didCarve = false;
                int maxAttempts = MaxRoomRetryCount;

                for (int j = 0; j < maxAttempts && !didCarve; j++)
                {
                    // TODO: Fix possible error where generated room is larger than the tilemap.
                    var room = rooms[i];

                    didCarve = CarveRoom(
                        tilemap.Grid,
                        room,
                        new Point2(
                            Random.NextInt(0, tilemap.Grid.Cols - room.Tiles.Cols),
                            Random.NextInt(0, tilemap.Grid.Rows - room.Tiles.Rows)));
                }
            }

            return tilemap;
        }
        
        /// <summary>
        ///  Carve a room into the tile map.
        /// </summary>
        public bool CarveRoom(Grid<Tile> map, RoomLayout room, Point2 topLeft)
        {
            // Check if room is overlapped another feature in the map.
            if (DoesOverlap(map, room.Tiles, topLeft))
            {
                return false;
            }

            // TODO: Copy this code into a Grid helper utility with tests.
            // Copy the room.
            for (int y = 0; y < room.Tiles.Rows; y++)
            {
                for (int x = 0; x < room.Tiles.Cols; x++)
                {
                    map[x + topLeft.X, y + topLeft.Y] = room.Tiles[x, y];
                }
            }

            return true;
        }

        public bool DoesOverlap(Grid<Tile> map, Grid<Tile> roomTiles, Point2 topLeft)
        {
            for (int y = 0; y < roomTiles.Rows; y++)
            {
                for (int x = 0; x < roomTiles.Cols; x++)
                {
                    var mapTile = map[x + topLeft.X, y + topLeft.Y];
                    
                    // TODO: Refactor this check, its gross.
                    if (mapTile.Type != EmptyTileId &&              // TODO: Use property .IsEmpty
                        !(mapTile.Type == WallTileId && roomTiles[x, y].Type == WallTileId))// Moar properties
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
