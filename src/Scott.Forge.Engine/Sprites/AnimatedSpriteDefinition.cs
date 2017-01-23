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
using Microsoft.Xna.Framework.Graphics;

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  AnimatedSpriteDefinition holds data describing 2d graphical image known as a Sprite. This class holds the
    ///  name, size, animations and initial texture atlas offset and other data for a simple sprite.
    /// </summary>
    /// <remarks>
    ///  Animated sprite definitions are composed of a sprite definition and animation definition.
    /// </remarks>
    public class AnimatedSpriteDefinition
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="spriteDefinition">Sprite definition.</param>
        /// <param name="animationDefinitions">Animation definitions.</param>
        public AnimatedSpriteDefinition(
            SpriteDefinition spriteDefinition,
            AnimationSetDefinition animationDefinitions)
        {
            if (spriteDefinition == null)
            {
                throw new ArgumentNullException("spriteDefinition");
            }

            if (animationDefinitions == null)
            {
                throw new ArgumentNullException("animationDefinitions");
            }

            Sprite = spriteDefinition;
            Animations = animationDefinitions;
        }

        /// <summary>
        ///  Get the sprite definition.
        /// </summary>
        public SpriteDefinition Sprite { get; private set; }

        /// <summary>
        ///  Get the animation definitions.
        /// </summary>
        public AnimationSetDefinition Animations { get; private set; }
    }
}
