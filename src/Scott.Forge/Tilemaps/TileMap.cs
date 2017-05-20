﻿/*
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scott.Forge.Tilemaps
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
        /// <param name="tileWidth">Width of each tile.</param>
        /// <param name="tileHeight">Height of each tile.</param>
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
        ///  Get the tile grid.
        /// </summary>
        public Grid<Tile> Grid { get; private set; }

        /// <summary>
        ///  Get the height of the tile map.
        /// </summary>
        public float Height { get { return TileHeight * Grid.Rows; } }

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
        ///  Get the index of the top left most tile that is visible to the camera.
        /// </summary>
        /// <param name="camera">Camera with visibility information.</param>
        /// <returns>The top left most visible tile.</returns>
        public Point2 GetTopLeftmostVisibleTile(Graphics.Camera camera)
        {
            var worldTopLeft = camera.ScreenToWorld(new Vector2(0, 0));
            return new Point2(
                MathHelper.Clamp((int) Math.Floor(worldTopLeft.X / TileWidth), 0, Grid.Cols - 1),
                MathHelper.Clamp((int) Math.Floor(worldTopLeft.Y / TileHeight), 0, Grid.Rows - 1));
        }

        /// <summary>
        ///  Get the index of the bottom right most tile that is visible to the camera.
        /// </summary>
        /// <remarks>
        ///  Due to tile boundaries it is possible for this method to return a tile that is exactly on the edge of
        ///  the screen (making the tile not visible). This will happen when the camera's top left position is
        ///  aligned with a tile's top left corner, and the viewport size is a multiple of the tile width or height.
        ///  
        ///  This will not break anything it just may cause the renderer to render a few extra tiles.
        /// </remarks>
        /// <param name="camera">Camera with visibility information.</param>
        /// <returns>Top bottom right most visible tile.</returns>
        public Point2 GetBottomRightmostVisibleTile(Graphics.Camera camera)
        {
            var screenBottomRight = new Vector2(camera.Viewport.Width, camera.Viewport.Height);
            var worldBottomRight = camera.ScreenToWorld(screenBottomRight);
            
            return new Point2(
                MathHelper.Clamp((int) Math.Ceiling(worldBottomRight.X / TileWidth), 0, Grid.Cols -1),
                MathHelper.Clamp((int) Math.Ceiling(worldBottomRight.Y / TileHeight), 0, Grid.Rows - 1));
        }
    }
}
