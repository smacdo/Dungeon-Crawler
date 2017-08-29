/*
 * Copyright 2012-2017 Scott MacDonald.
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
using Scott.Forge.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;

namespace Scott.Forge.Sprites
{
    /// <summary>
    ///  Loads an animated sprite from an XML input stream and returns an AnimatedSpriteDefinition object.
    ///  
    ///  TODO: Rewrite the loader with a forward xml reader to improve performance. Current version makes heavy use of
    ///        LINQ which will cause perf/memory issues on games with large numbers of sprites or low end devices.
    /// </summary>
    //[ContentReader(typeof(AnimatedSpriteDefinition), ".sprite")]
    public class AnimatedSpriteXmlReader : IContentReader<AnimatedSpriteDefinition>
    {
        private string _assetPath;

        /// <summary>
        ///  Read an animated sprite from XML and convert it into an AnimatedSpriteDefinition object.
        /// </summary>
        /// <param name="inputStream">Stream to read serialized asset data from.</param>
        /// <param name="assetPath">Relative path to the serialized asset.</param>
        /// <param name="content">Content manager.</param>
        /// <returns>Deserialized content object.</returns>
        public Task<AnimatedSpriteDefinition> Read(
            Stream inputStream,
            string assetPath,
            IContentManager content)
        {
            // Check arguments for validty.
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentNullException(nameof(assetPath));
            }

            _assetPath = assetPath;

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            // Read and convert the XML document into an animation definition.
            var xmlDoc = XDocument.Load(inputStream);
            var rootNode = xmlDoc.Root;

            if (rootNode.Name.LocalName != "sprite")
            {
                throw new ContentLoadException(_assetPath, "Animated sprite does not have root sprite element");
            }

            return null;
        }
    }
}
