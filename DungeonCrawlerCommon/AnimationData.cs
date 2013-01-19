﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Scott.Dungeon.Data
{
    /// <summary>
    /// Information about a sprite animation
    /// </summary>
    public class AnimationData
    {
        private const int DIRECTION_COUNT = 4;
        private const float DEFAULT_FRAME_TIME = 0.1f;

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
        public AnimationData( string name, float frameTime, List<List<Rectangle>> directions )
        {
            Name = name;
            FrameTime = frameTime;

            Directions = new List<List<Rectangle>>( DIRECTION_COUNT );

            // Copy all of the animations for each direction
            for ( int i = 0; i < DIRECTION_COUNT; ++i )
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
        public Rectangle GetSpriteRectFor( Direction direction, int frame )
        {
            return Directions[(int) direction][frame];
        }

        public List<Rectangle> GetFramesList( Direction direction )
        {
            return new List<Rectangle>( Directions[(int) direction] );
        }

        public List<Rectangle>.Enumerator GetFramesEnumerator( Direction direction )
        {
            return Directions[(int) direction].GetEnumerator();
        }
    }
}
