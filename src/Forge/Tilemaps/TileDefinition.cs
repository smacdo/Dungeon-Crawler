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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Tilemaps
{
    /// <summary>
    ///  Contains information about a type of tile as defined by the tile's unique id.
    /// </summary>
    public class TileDefinition
    {
        private readonly ushort mId = 0;
        
        /// <summary>
        ///  Tile constructor.
        /// </summary>
        /// <param name="id">Unique tile id.</param>
        /// <param name="name">Tile name.</param>
        /// <param name="atlasX">Texture atlas X offset.</param>
        /// <param name="atlasY">Texture atlas Y offset.</param>
        /// <param name="color">Color of the tile. (TODO: TEMP; REMOVE).</param>
        public TileDefinition(ushort id, string name, int atlasX, int atlasY)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (atlasX < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(atlasX), "Texture atlas x position cannot be negative");
            }

            if (atlasY < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(atlasY), "Texture atlas y position cannot be negative");
            }

            mId = id;
            Name = name;
            AtlasX = atlasX;
            AtlasY = atlasY;
        }

        /// <summary>
        ///  Get or set unique id for the tile.
        /// </summary>
        public ushort Id { get { return mId; } }

        /// <summary>
        ///  Get or set a name for the tile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Get or set the texture atlas x position.
        /// </summary>
        public int AtlasX { get; set; }

        /// <summary>
        ///  Get or set the texture atlas y position.
        /// </summary>
        public int AtlasY { get; set; }

        /// <summary>
        ///  Get or set extra flags that can be used by the game.
        /// </summary>
        public ulong ExtraFlags { get; set; } = 0;

        /// <summary>
        ///  Get a dictionary of additional properties for a tile.
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();
    }
}
