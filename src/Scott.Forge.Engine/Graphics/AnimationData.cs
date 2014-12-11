/*
 * Copyright 2012-2014 Scott MacDonald
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

namespace Scott.Forge.Engine.Graphics
{
    /// <summary>
    ///  Contains runtime information describing a set of named animations built from static
    ///  images contained in one or more sprite atlases. This class should be directly consumed by
    ///  the SpriteData class, and read/written from disk.
    ///  
    ///  TODO: make this class internal.
    /// </summary>
    public class AnimationData      // TODO: Rename AnimationDefinition
    {
        private const int DirectionCount = 4;
        private const float DefaultFrameTime = 0.10f;

        /// <summary>
        /// Name of the animation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount of time to play each frame in the animation
        /// </summary>
        public float FrameTime { get; set; }

        /// <summary>
        /// A list of animation directions. Each direction contains a list of rectangles that
        /// specify the offset for each sprite frame in the animation.
        /// 
        /// Each direction is assumed to have the same number of sprite frames. If this not the
        /// case then undefined results may occur!
        /// </summary>
        public List< List<Rectangle> > Directions;

        /// <summary>
        /// The number of frames in an animation.
        /// </summary>
        public int FrameCount
        {
            get
            {
                return Directions[0].Count;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="frameTime">Amount of time to display each frame</param>
        internal AnimationData( string name, float frameTime, List<List<Rectangle>> directions )
        {
            Name = name;
            FrameTime = frameTime;

            Directions = new List<List<Rectangle>>( DirectionCount );

            // Copy all of the animations for each direction
            for ( int i = 0; i < DirectionCount; ++i )
            {
                Directions.Add( new List<Rectangle>( directions[i] ) );
            }
        }

        /// <summary>
        /// Returns the requested sprite frame
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Rectangle GetSpriteRectFor( DirectionName direction, int frame )
        {
            return Directions[(int) direction][frame];
        }

        public List<Rectangle> GetFramesList( DirectionName direction )
        {
            return new List<Rectangle>( Directions[(int) direction] );
        }

        public List<Rectangle>.Enumerator GetFramesEnumerator( DirectionName direction )
        {
            return Directions[(int) direction].GetEnumerator();
        }
    }

    /// <summary>
    /// The action to perform when an action has ended
    /// </summary>
    public enum AnimationEndingAction
    {
        Stop,         // Freeze on the last played animation frame
        Loop,         // Restart on animation frame zero and continue animating
        StopAndReset, // Freeze on the first animation frame
    }
}
