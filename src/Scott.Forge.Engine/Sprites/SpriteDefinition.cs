/*
 * Copyright 2012-2015 Scott MacDonald.
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
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  SpriteDefinition holds data describing 2d graphical image known as a "Sprite". This class is responsible for
    ///  holding basic sprite data (Such as name, width, height, etc), a list of animations that the sprite is capable
    ///  of performing and the information required for displaying each image frame.
    /// </summary>
    /// <remarks>
    ///  SpriteData holds one or more images, and these images are stored in one or more texture atlases. SpriteData
    ///  describes how to index the images stored in the texture atlas. Generally SpriteData should not be modified at
    ///  run time. It should be modified when it is stored on disk, and then read into memory.
    /// </remarks>
    public class SpriteDefinition
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="name">Name of the sprite.</param>
        /// <param name="texture">Texture atlas that contains this sprite.</param>
        /// <param name="defaultAnimation">Default animation name.</param>
        /// <param name="defaultDirection">Default direction to use when animating.</param>
        /// <param name="animationList">List of animation definitions to include with the sprite.</param>
        public SpriteDefinition(
            string name,
            Texture2D texture,
            string defaultAnimation,
            DirectionName defaultDirection,
            List<AnimationDefinition> animationList)
        {
            // Check arguments for errors.
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            if (string.IsNullOrWhiteSpace(defaultAnimation))
            {
                throw new ArgumentNullException("defaultAnimation");
            }

            if (animationList == null)
            {
                throw new ArgumentNullException("animationList");
            }
            else if (animationList.Count < 1)
            {
                throw new ArgumentException("Must have at least one animation", "animationList");
            }

            // Copy properties.
            Name = name;
            Texture = texture;
            OriginOffset = Vector2.Zero;

            // Copy animations
            Animations = new Dictionary<string, AnimationDefinition>(animationList.Count);

            foreach (AnimationDefinition animation in animationList)
            {
                Animations.Add(animation.Name, animation);
            }

            // Verify default animation and direction exists before copying default animation properties.
            if (!Animations.ContainsKey(defaultAnimation))
            {
                throw new ArgumentException("Invalid default animation name", "defaultAnimation");
            }

            if (Animations[defaultAnimation].DirectionCount < (int) defaultDirection)
            {
                throw new ArgumentException("Invalid default direction", "defaultDirection");
            }

            DefaultAnimationName = defaultAnimation;
            DefaultAnimationDirection = defaultDirection;
            DefaultAnimation = Animations[DefaultAnimationName];
        }


        /// <summary>
        ///  Get a list of sprite animations.
        /// </summary>
        public Dictionary<string, AnimationDefinition> Animations { get; private set; }

        /// <summary>
        ///  Get the default sprite animation.
        /// </summary>
        public AnimationDefinition DefaultAnimation { get; private set; }

        /// <summary>
        ///  Get the default sprite animation direction.
        /// </summary>
        public DirectionName DefaultAnimationDirection { get; private set; }

        /// <summary>
        ///  Get the default sprite animation name.
        /// </summary>
        public string DefaultAnimationName { get; private set; }

        /// <summary>
        ///  Get the name of the sprite.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///  Get the offset from local origin when rendering.
        /// </summary>
        public Vector2 OriginOffset { get; private set; }

        /// <summary>
        ///  Get the sprite texture atlas.
        /// </summary>
        public Texture2D Texture { get; private set; }
    }
}
