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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  SpriteDefinition defines the data required to display a traditional 2d sprite stored in a texture atlas.
    /// </summary>
    public class SpriteDefinition
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="name">Name of the sprite.</param>
        /// <param name="texture">Texture atlas that contains this sprite.</param>
        /// <param name="size">Width and height of the sprite.</param>
        /// <param name="startingOffset">The X and Y atlas offset to use if the sprite is not animating.</param>
        public SpriteDefinition(
            string name,
            SizeF size,
            Vector2 startingOffset,
            Texture2D texture)
        {
            // Check arguments for errors.
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (size.Width <= 0)
            {
                throw new ArgumentException("Sprite width must be larger than zero", "size");
            }
            else if (size.Height <= 0)
            {
                throw new ArgumentException("Sprite height must be larger than zero", "size");
            }

            if (startingOffset.X < 0)
            {
                throw new ArgumentException("Sprite atlas x offset must be at least zero", "startingOffset");
            }
            else if (startingOffset.Y < 0)
            {
                throw new ArgumentException("Sprite atlas y offset must be at least zero", "startingOffset");
            }

            // Copy properties.
            Name = name;
            Texture = texture;
            StartingOffset = startingOffset;
            Size = size;
        }

        /// <summary>
        ///  Get the name of the sprite.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///  Get the size of the sprite. (X is width, and Y is height).
        /// </summary>
        public SizeF Size { get; private set; }

        /// <summary>
        ///  Get the X and Y atlas offset to use if the sprite is not animating.
        /// </summary>
        public Vector2 StartingOffset { get; private set; }

        /// <summar>y
        ///  Get the sprite texture atlas.
        /// </summary>
        public Texture2D Texture { get; private set; }
    }
}
