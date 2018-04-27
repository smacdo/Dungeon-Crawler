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
using Forge.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forge.Spatial;

namespace Forge.Tilemaps
{
    /// <summary>
    ///  A spatial grid of tiles.
    /// </summary>
    public class TileMap
    {
        /// <summary>
        ///  Create a new empty Tilemap.
        /// </summary>
        /// <param name="tileset">Tile set to use in the tile map.</param>
        /// <param name="cols">Number of columns in the tile map.</param>
        /// <param name="rows">Number of rows in the tile map.</param>
        public TileMap(TileSet tileset, int cols, int rows)
        {
            if (tileset == null)
            {
                throw new ArgumentNullException(nameof(tileset));
            }

            if (cols < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cols), "Cols must be larger than zero");
            }

            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be larger than zero");
            }

            TileSet = tileset;
            Grid = new Grid<Tile>(cols, rows);
        }

        /// <summary>
        ///  Construct a tile map an existing grid.
        /// </summary>
        /// <param name="tileset">Tile set to use.</param>
        /// <param name="tiles">Grid of tiles to use.</param>
        public TileMap(TileSet tileset, Grid<Tile> tiles)
        {
            if (tileset == null)
            {
                throw new ArgumentNullException(nameof(tileset));
            }

            if (tiles == null)
            {
                throw new ArgumentNullException(nameof(tiles));
            }

            TileSet = tileset;
            Grid = tiles;
        }

        /// <summary>
        ///  Get the number of columns in the tile map.
        /// </summary>
        public int Cols { get { return Grid.Cols; } }

        /// <summary>
        ///  Get the tile grid.
        /// </summary>
        public Grid<Tile> Grid { get; private set; }

        /// <summary>
        ///  Get the height of the tile map.
        /// </summary>
        public float Height { get { return TileHeight * Grid.Rows; } }

        /// <summary>
        ///  Get the number of rows in the tile map.
        /// </summary>
        public int Rows { get { return Grid.Rows; } }

        /// <summary>
        ///  Get the height of a tile.
        /// </summary>
        public float TileHeight { get { return TileSet.TileHeight; } }

        /// <summary>
        ///  Get the width of a tile.
        /// </summary>
        public float TileWidth { get { return TileSet.TileWidth; } }

        /// <summary>
        ///  Get the tile set used with this tile map.
        /// </summary>
        public TileSet TileSet { get; private set; }

        /// <summary>
        ///  Get the width of the tile map.
        /// </summary>
        public float Width { get { return TileWidth * Grid.Cols; } }

        /// <summary>
        ///  Get the world position for a tile point.
        /// </summary>
        /// <remarks>
        ///  The tile origin is in the center instead of the top left corner.
        /// </remarks>
        /// <param name="tilePoint">X/Y (column row) of the tile.</param>
        /// <returns>Tile center in world coordinate space.</returns>
        public Vector2 GetWorldPositionForTile(Point2 tilePoint)
        {
            return new Vector2(
                TileWidth / 2 + tilePoint.X * TileWidth,
                TileHeight / 2 + tilePoint.Y * TileHeight);
        }
    }
}
