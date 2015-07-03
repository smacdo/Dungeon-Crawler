/*
 * Copyright 2012-2015 Scott MacDonald
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

namespace Scott.Forge.Engine.Sprites
{
    /// <summary>
    ///  Contains runtime information describing a set of named animations built from static images contained in one or
    ///  more sprite atlases. This class should be directly consumed by SpriteDefinition, and read/written from disk.
    /// </summary>
    /// <remarks>
    ///  Built to be a readonly class.
    ///  All directions must have the same frame count.
    /// </remarks>
    public class AnimationDefinition
    {
        private const int MaxDirectionCount = 4;
        private const float DefaultFrameTime = 0.10f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frameTime">Amount of time to display each frame</param>
        /// <param name="directions">List of animated sprite frames grouped by sprite direction.</param>
        public AnimationDefinition(string name, float frameTime, List<List<RectF>> directions)
        {
            // Check arguments for errors.
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (frameTime <= 0.0f)
            {
                throw new ArgumentException("Frame time must be larger than zero", "frameTime");
            }

            if (directions == null)
            {
                throw new ArgumentNullException("directions");
            }

            if (directions.Count != 1 && directions.Count != MaxDirectionCount)
            {
                throw new ArgumentException("Must have either 1 or 4 directions defined", "directions");
            }
            
            // Copy properties.
            Name = name;
            FrameTime = frameTime;
            FrameCount = directions[0].Count;
            DirectionCount = directions.Count;
            Directions = new RectF[DirectionCount, FrameCount];

            // Check frameCount > 0.
            if (FrameCount < 1)
            {
                throw new ArgumentException("Sprite animation must have at least one frame");
            }

            // Copy all of the animations for each direction
            for (int i = 0; i < DirectionCount; ++i)
            {
                // Check frame count is the same as previous frames.
                if (directions[i].Count != FrameCount)
                {
                    throw new ArgumentException("Sprite animation frame count is not consistent", "directions");
                }

                for (int j = 0; j < directions[i].Count; ++j)
                {
                    Directions[i, j] = directions[i][j];
                }
            }
        }

        /// <summary>
        ///  Get a list of sprite animation frames grouped by direction.
        /// </summary>
        /// <remarks>
        ///  A list of animation directions. Each direction contains a list of rectangles that specify the offset for
        ///  each sprite frame in the animation. Each direction is assumed to have the same number of sprite frames.
        ///  If this not the case then undefined results may occur!
        /// </summary>
        public readonly RectF[,] Directions;

        /// <summary>
        ///  Get the number of directions defined for this animation.
        /// </summary>
        public int DirectionCount { get; private set; }

        /// <summary>
        ///  Get the number of frames in an animation.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        ///  Get the amount of time to play each frame in the animation.
        /// </summary>
        public float FrameTime { get; private set; }

        /// <summary>
        /// Get the name of the animation
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///  Get the requested animated sprite frame.
        /// </summary>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="frame">Frame animation index.</param>
        /// <returns>Sprite frame atlas.</returns>
        public RectF GetSpriteRectFor(DirectionName direction, int frame)
        {
            return Directions[(int) direction, frame];
        }
    }
}