/*
 * Copyright 2017 Scott MacDonald
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
using Scott.Forge.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Scott.Forge.Content;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Loads Tiled .TMX map files.
    /// </summary>
    [ContentReader(typeof(TileMap), ".tmx")]
    internal class TiledMapReader : ContentReader<TileMap>
    {
        /// <summary>
        ///  Read a serialized asset from an input stream and return it as a loaded object.
        /// </summary>
        /// <param name="inputStream">Stream to read serialized asset data from.</param>
        /// <param name="assetPath">Relative path to the serialized asset.</param>
        /// <param name="content">Content manager.</param>
        /// <returns>Deserialized content object.</returns>
        public override TileMap Read(
            Stream inputStream,
            string assetPath,
            IContentManager content)
        {
            var xml = new XmlDocument();
            xml.Load(inputStream);

            return ReadMap(xml.SelectSingleNode("/map"), content);
        }

        private TileMap ReadMap(XmlNode mapNode, IContentManager content)
        {
            if (mapNode == null)
            {
                throw new ArgumentNullException(nameof(mapNode));
            }

            // Check map type is orthogonal - no other type is supported at the moment.
            var mapType = mapNode.Attributes["orientation"]?.Value;

            if (mapType != "orthogonal")
            {
                throw new InvalidOperationException("Map format must be orthogonal, other types not supported");
            }

            // Initialize the tile grid and tile map after reading attributes describing the map.
            var colCount = Convert.ToInt32(mapNode.Attributes["width"]?.Value);
            var rowCount = Convert.ToInt32(mapNode.Attributes["height"]?.Value);
            var tileWidth = Convert.ToInt32(mapNode.Attributes["tilewidth"]?.Value);
            var tileHeight = Convert.ToInt32(mapNode.Attributes["tileheight"]?.Value);

            var tileGrid = new Grid<Tile>(colCount, rowCount);
            var tileSet = new TileSet(null, tileWidth, tileHeight);         // TODO: This null needs to be dealt with.
            var tileMap = new TileMap(tileSet, tileGrid);

            return tileMap;
        }

        // TODO: Support internal and external TileSet.
        // TODO: Move this to a TileSet content reader.
        public TileSet ReadExternalTileSet(XmlNode root, IContentManager content)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var name = root.Attributes["name"]?.Value;      // TODO: Not null.

            var firstTileId = Convert.ToInt32(root.Attributes["firstgid"]?.Value);
            var tileWidth = Convert.ToInt32(root.Attributes["tilewidth"]?.Value);
            var tileHeight = Convert.ToInt32(root.Attributes["tileheight"]?.Value);
            var spacing = Convert.ToInt32(root.Attributes["spacing"]?.Value);
            var margin = Convert.ToInt32(root.Attributes["margin"]?.Value);
            var columns = Convert.ToInt32(root.Attributes["columns"]?.Value);
            var tileCount = Convert.ToInt32(root.Attributes["tilecount"]?.Value);

            return null;
        }
    }
}
