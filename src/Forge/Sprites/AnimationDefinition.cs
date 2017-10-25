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

namespace Forge.Sprites
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
        public const float DefaultFrameTimeInSeconds = 0.10f;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frameSeconds">Amount of time in seconds display each frame</param>
        /// <param name="atlasPositions">List of sprite frames organized by direction.</param>
        public AnimationDefinition(string name, float frameSeconds, SpriteFrame[,] frames)
        {
            // Assign properties if arguments are valid.
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;

            if (frameSeconds <= 0.0f)
            {
                throw new ArgumentException("Frame time must be larger than zero", nameof(frameSeconds));
            }

            FrameSeconds = frameSeconds;

            if (frames == null)
            {
                throw new ArgumentNullException(nameof(frames));
            }

            FrameCount = frames.GetLength(1);

            if (FrameCount < 1)
            {
                throw new InvalidOperationException("Animation must have at least one frame");
            }

            // Copy the frames atlas offset array. If only one direction is provided, duplicate the frames for the
            // missing directions.
            _frames = new SpriteFrame[Constants.DirectionCount, FrameCount];

            int sourceDirectionCount = frames.GetLength(0);
            
            if (sourceDirectionCount == Constants.DirectionCount)
            {
                Array.Copy(frames, _frames, frames.Length);
            }
            else if (sourceDirectionCount == 1)
            {
                // Populate missing directions with frames from first direction.                
                for (var i = 0; i < Constants.DirectionCount; i++)
                {
                    for (var j = 0; j < FrameCount; j++)
                    {
                        _frames[i, j] = frames[0,j];
                    }
                }
            }
            else
            {
                throw new ArgumentException("Must have either 1 or 4 directions defined", nameof(frames));
            }
        }

        /// <summary>
        ///  Get a list of sprite animation frames grouped by direction.
        /// </summary>
        /// <remarks>
        ///  A list of animation directions. Each direction contains a list of (X,Y) values that specify the offset for
        ///  each sprite frame in the animation. Each direction is assumed to have the same number of sprite frames.
        /// </summary>
        private SpriteFrame[,] _frames;

        /// <summary>
        ///  Get the number of frames in an animation.
        /// </summary>
        public int FrameCount { get; }

        /// <summary>
        ///  Get the amount of time to play each frame in the animation.
        /// </summary>
        public float FrameSeconds { get; }     // TODO: Use TimeSpan.

        /// <summary>
        /// Get the name of the animation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  Get the upper left corner of a sprite for a frame in a given direction.
        /// </summary>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>Sprite atlas top left.</returns>
        public Vector2 GetAtlasPosition(DirectionName direction, int frame)
        {
            return _frames[(int) direction, frame].AtlasPosition;
        }

        /// <summary>
        ///  Get events associated with a frame in a given direction.
        /// </summary>
        /// <param name="direction">Sprite direction.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>Animation events (possibly null).</returns>
        public AnimationEvent[] GetEvents(DirectionName direction, int frame)
        {
            return _frames[(int)direction, frame].Events;
        }
    }
    
    /// <summary>
    ///  A single frame of animation in a sprite.
    /// </summary>
    public struct SpriteFrame
    {
        /// <summary>
        ///  The upper left corner position of sprite in texture atlas
        /// </summary>
        public Vector2 AtlasPosition;

        /// <summary>
        ///  A list (possibly null) of events associated with this sprite frame.
        /// </summary>
        public AnimationEvent[] Events;
    }
}