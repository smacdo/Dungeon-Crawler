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
    ///  Holds data for a single tile on a tile map.
    /// </summary>
    public struct Tile
    {
        /// <summary>
        ///  Tile  constructor.
        /// </summary>
        /// <param name="type">Tile type id.</param>
        public Tile(ushort type)
        {
            Type = type;
            Data = 0;
            Collision = 0;
        }

        /// <summary>
        ///  Get or set the tile type id.
        /// </summary>
        public ushort Type { get; set; }

        /// <summary>
        ///  Get or set collision data.
        /// </summary>
        public byte Collision { get; set; }

        /// <summary>
        ///  Get or set data associated with this tile.
        /// </summary>
        public ushort Data { get; set; }

        public bool HasFlag(ushort flag)
        {
            return (Data & flag) != 0;
        }

        public void SetFlag(ushort flag, bool value)
        {
            if (value)
            {
                Data |= flag;
            }
            else
            {
                Data &= flag;
            }
        }

        public void SetImpassable()
        {
            Collision = 255;
        }
    }
}
