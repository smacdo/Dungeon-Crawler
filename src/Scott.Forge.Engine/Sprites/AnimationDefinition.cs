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
        private const float DefaultFrameTime = 0.10f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frameSeconds">Amount of time in seconds display each frame</param>
        /// <param name="atlasPositions">Upper left corner position of sprite in texture atlas.</param>
        public AnimationDefinition(string name, float frameSeconds, List<List<Vector2>> atlasPositions)
        {
            // Check arguments for errors.
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (frameSeconds <= 0.0f)
            {
                throw new ArgumentException("Frame time must be larger than zero", nameof(frameSeconds));
            }

            if (atlasPositions == null)
            {
                throw new ArgumentNullException(nameof(atlasPositions));
            }

            // TODO: Can we remove restriction on either 1 or zero? Would be nice to have just > 0.
            if (atlasPositions.Count != 1 && atlasPositions.Count != Constants.DirectionCount)
            {
                throw new ArgumentException("Must have either 1 or 4 directions defined", nameof(atlasPositions));
            }
            
            // Copy properties.
            Name = name;
            FrameSeconds = frameSeconds;
            FrameCount = atlasPositions[0].Count;
            Frames = new Vector2[Constants.DirectionCount, FrameCount];

            // Check frameCount > 0.
            if (FrameCount < 1)
            {
                throw new ArgumentException("Sprite animation must have at least one frame", nameof(atlasPositions));
            }

            // Copy all of the animations for each direction. Duplicate animation data if only one direction was
            // provided.
            for (int i = 0; i < Constants.DirectionCount; ++i)
            { 
                // Copy frames if there are four directions defined, otherwise duplicate the first direction.
                int directionIndex = (atlasPositions.Count == 1 ? 0 : i);

                if (atlasPositions[directionIndex].Count != FrameCount)
                {
                    throw new ArgumentException("Sprite animation frame count is not consistent", nameof(atlasPositions));
                }

                for (int j = 0; j < atlasPositions[directionIndex].Count; ++j)
                {
                    Frames[i, j] = atlasPositions[directionIndex][j];
                }
            }
        }

        /// <summary>
        ///  Get a list of sprite animation frames grouped by direction.
        /// </summary>
        /// <remarks>
        ///  A list of animation directions. Each direction contains a list of (X,Y) values that specify the offset for
        ///  each sprite frame in the animation. Each direction is assumed to have the same number of sprite frames.
        /// </summary>
        public readonly Vector2[,] Frames;

        /// <summary>
        ///  Get the number of frames in an animation.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        ///  Get the amount of time to play each frame in the animation.
        /// </summary>
        public float FrameSeconds { get; private set; }

        /// <summary>
        /// Get the name of the animation.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///  Get the requested animated sprite frame.
        /// </summary>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="frame">Frame animation index.</param>
        /// <returns>Sprite frame atlas.</returns>
        public Vector2 GetAtlasPosition(DirectionName direction, int frame)
        {
            return Frames[(int) direction, frame];
        }
    }
}