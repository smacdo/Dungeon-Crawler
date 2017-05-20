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
            for (int i = 0; i < 50; i++)
            {
                AddRandomRoom(tilemap.Grid);    
            }

            return tilemap;
        }

        public void AddRandomRoom(Grid<Tile> grid)
        {
            var cols = Random.NextInt(4, 24);      // TODO: Make configurable + weighted.
            var rows = Random.NextInt(4, 24);     // TODO: Make configurable + weighted.

            var xEnd = grid.Cols - cols;
            var yEnd = grid.Rows - rows;
            
            if (xEnd <= cols)
            {
                throw new InvalidOperationException();
            }   

            if (yEnd <= rows)
            {
                throw new InvalidOperationException();
            }

            var x = Random.NextInt(0, xEnd);
            var y = Random.NextInt(0, yEnd);

            CarveRoom(
                grid,
                x,
                y,
                cols,
                rows,
                FloorTileId,
                WallTileId);
        }
        
        /// <summary>
        ///  Carve a room into the tile map.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <param name="floor"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public void CarveRoom(
            Grid<Tile> grid,
            int left, 
            int top,
            int cols,
            int rows,
            ushort floor,
            ushort wall)
        {
            // Quickly carve something and improve this later.
            grid.Fill(left, top, cols, rows, (Grid<Tile> g, int x, int y) =>
            {
                return new Tile(floor);
            });
            
            // Carve walls but only if there is empty space.
        }
    }
}
