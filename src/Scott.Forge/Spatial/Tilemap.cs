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

namespace Scott.Forge.Spatial
{
    /// <summary>
    ///  A spatial grid of tiles.
    /// </summary>
    public class Tilemap
    {
        public Tilemap(int cols, int rows, float tileWidth, float tileHeight)
        {
            // TODO: Test inputs.
            Grid = new Grid<Tile>(cols, rows);

            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        public Grid<Tile> Grid { get; private set; }

        public float TileWidth { get; private set; }

        public float TileHeight { get; private set; }
    }
}
