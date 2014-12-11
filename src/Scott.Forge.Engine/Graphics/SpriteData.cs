/*
 * Copyright 2012-2013 Scott MacDonald.
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Forge.Engine.Graphics
{
    /// <summary>
    ///  SpriteData holds data describing 2d graphical image known as a "Sprite". This class is
    ///  responsible for holding basic sprite data (Such as name, width, height, etc), a list of
    ///  animations that the sprite is capable of performing and the information required for
    ///  displaying each image frame.
    /// </summary>
    /// <remarks>
    ///  SpriteData holds one or more images, and these images are stored in one or more texture
    ///  atlases. SpriteData describes how to index the images stored in the texture atlas.
    ///  
    ///  Generally SpriteData should not be modified at run time. It should be modified when it is
    ///  stored on disk, and then read into memory.
    ///  
    ///  TODO: Make this an internal class. Make a struct Sprite class.
    /// </remarks>
    /// [ContentImporter( ".sprite", DisplayName = "Sprite Xml Importer", DefaultProcessor = "SpriteContentProcessor" )
    public class SpriteData         // TODO: Rename SpriteDefinition
    {
        /// <summary>
        /// Name of the sprite
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sprite's texture atlas
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// List of animations for this sprite
        /// </summary>
        public Dictionary<string, AnimationData> Animations { get; set; }

        public AnimationData DefaultAnimation { get; private set; }

        /// <summary>
        /// The default animation name for this sprite
        /// </summary>
        public string DefaultAnimationName { get; set; }

        /// <summary>
        /// Direction for the default animation
        /// </summary>
        public DirectionName DefaultAnimationDirection { get; set; }

        /// <summary>
        /// Offset from local origin when rendering
        /// </summary>
        public Vector2 OriginOffset { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="texture">Texture atlas that contains this sprite</param>
        /// <param name="defaultAnimation">Default animation name</param>
        /// <param name="defaultDirection">Default direction to use when animating without a direction</param>
        internal SpriteData( string name,
                             Texture2D texture,
                             string defaultAnimation,
                             DirectionName defaultDirection,
                             List<AnimationData> animationList )
        {
            Name = name;
            Texture = texture;
            
            // Copy animations
            Animations = new Dictionary<string, AnimationData>( animationList.Count );

            foreach ( AnimationData animation in animationList )
            {
                Animations.Add( animation.Name, animation );
            }

            DefaultAnimationName = defaultAnimation;
            DefaultAnimationDirection = defaultDirection;
            DefaultAnimation = Animations[DefaultAnimationName];
            OriginOffset = Vector2.Zero;
        }
    }
}
