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
using System;
using System.Collections.Generic;
using Forge;
using Forge.Random;
using Forge.Tilemaps;
using Scott.DungeonCrawler.Levels;
using System.Linq;

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
        ///  Get or set the tile definition for empty tiles.
        /// </summary>
        public TileDefinition Void { get; set; }

        /// <summary>
        ///  Get or set the tile definition for walls.
        /// </summary>
        public TileDefinition Wall { get; set; }

        /// <summary>
        ///  Get or set the tile definition for floors.
        /// </summary>
        public TileDefinition Floor { get; set; }
        
        /// <summary>
        ///  Get or set the tile definition for floors.
        /// </summary>
        public TileDefinition Doorway { get; set; }


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
        /// <param name="dungeonCols">Maximum number of columns in the tile map.</param>
        /// <param name="dungeonRows">Maximum number of rows in the tile map.</param>
        /// <returns>A new tile map.</returns>
        public DungeonLevel Generate(int dungeonCols, int dungeonRows)
        {
            var builder = new DungeonBuilder(dungeonCols, dungeonRows, TileSet, Void);

            // Create new rooms in random locations.
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
            var roomCenters = new List<Point2>(rooms.Count);

            for (int i = 0; i < rooms.Count; i++)
            {
                bool didCarve = false;
                int maxAttempts = MaxRoomRetryCount;

                for (int j = 0; j < maxAttempts && !didCarve; j++)
                {
                    // TODO: Fix possible error where generated room is larger than the tilemap.
                    var room = rooms[i];
                    var topLeft = new Point2(
                        Random.NextInt(0, dungeonCols - room.Tiles.Cols),
                        Random.NextInt(0, dungeonRows - room.Tiles.Rows));

                    didCarve = builder.TryCarveRoom(room, topLeft);

                    if (didCarve)
                    {
                        roomCenters.Add(room.SafeCenter + topLeft);
                    }
                }
            }

            // Connect each room to a room that was generated after it. This will ensure all rooms are connected.
            var hallwayGenerator = new HallGenerator(builder.Grid, Floor, Wall);

            for (int i = 1; i < roomCenters.Count; i++)
            {
                builder.CarveHallway(
                    hallwayGenerator.CreatePath(roomCenters[i], roomCenters[i - 1]),
                    Floor,
                    Wall,
                    Doorway);
            }

            // Create a new dungeon level holding the generated dungeon information.
            // TODO: Create player spawn zone (stairs up), finish (stairs down) and enemy spawn locations.
            return new DungeonLevel()
            {
                TileMap = builder.Finalize(),
                StairsUpPoint = roomCenters.First(),
                SpawnPoints = roomCenters.Skip(1).ToList()
            };
        }
    }
}
