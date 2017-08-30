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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Forge.Tilemaps
{
    /// <summary>
    ///  Holds a set of tile definitions indexed by their id and the info required to draw them on the screen.
    /// </summary>
    public class TileSet : Dictionary<ushort, TileDefinition>
    {
        private const int DefaultCapacity = 100;

        /// <summary>
        ///  Tile set constructor.
        /// </summary>
        /// <param name="atlas">Texture atlas to use when drawing.</param>
        /// <param name="tileWidth">Width of a tile, in pixels.</param>
        /// <param name="tileHeight">Height of a tile, in pixels.</param>
        public TileSet(Texture2D atlas, float tileWidth, float tileHeight)
            : this(atlas, tileWidth, tileHeight, DefaultCapacity)
        {
        }

        /// <summary>
        ///  Tile set constructor with initial capacity.
        /// </summary>
        /// <param name="atlas">Texture atlas to use when drawing.</param>
        /// <param name="tileWidth">Width of a tile, in pixels.</param>
        /// <param name="tileHeight">Height of a tile, in pixels.</param>
        /// <param name="capacity">Inital capacity for tileset.</param>
        public TileSet(Texture2D atlas, float tileWidth, float tileHeight, int capacity)
            : base(capacity)
        {
            if (tileWidth < 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(tileWidth), "Tile width must be one or larger");
            }

            if (tileHeight < 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(tileHeight), "Tile height must be one or larger");
            }

            Atlas = atlas;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        /// <summary>
        ///  TileSet constructor.
        /// </summary>
        /// <param name="atlas">Texture atlas to use when drawing.</param>
        /// <param name="tileWidth">Width of a tile, in pixels.</param>
        /// <param name="tileHeight">Height of a tile, in pixels.</param>
        /// <param name="tiles">Collection of tiles to initialize this tileset with.</param>
        public TileSet(Texture2D atlas, float tileWidth, float tileHeight, ICollection<TileDefinition> tiles)
            : this(atlas, tileWidth, tileHeight, tiles != null ? tiles.Count : 0)
        {
            if (tiles == null)
            {
                throw new ArgumentNullException(nameof(tiles));
            }

            foreach (var tile in tiles)
            {
                Add(tile.Id, tile);
            }
        }

        /// <summary>
        ///  Get or set the texture atlas that is used to draw this tileset.
        /// </summary>
        public Texture2D Atlas { get; set; }

        /// <summary>
        ///  Height of a tile.
        /// </summary>
        public float TileHeight { get; set; }

        /// <summary>
        ///  Width of a tile.
        /// </summary>
        public float TileWidth { get; set; }

        /// <summary>
        ///  Add a tile definition to the tile set.
        /// </summary>
        /// <param name="tile">Tile definition to add.</param>
        public void Add(TileDefinition tile)
        {
            Add(tile.Id, tile);
        }

        /// <summary>
        ///  Add a tile definition to the tile set.
        /// </summary>
        /// <param name="tileId">Id of the tile to add. (Must match tile.Id)</param>
        /// <param name="tile">Tile definition to add.</param>
        public new void Add(ushort tileId, TileDefinition tile)
        {
            if (tile == null)
            {
                throw new ArgumentNullException(nameof(tile));
            }

            if (tileId != tile.Id)
            {
                throw new ArgumentException("Tile id must match tile.Id", nameof(tileId));
            }

            base.Add(tileId, tile);
        }
    }
}
