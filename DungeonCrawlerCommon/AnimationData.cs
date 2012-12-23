using System;
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
        /// <summary>
        /// Name of the animation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of the frames that should be played for this animation. Each
        /// frame is a rectangle offset to the sprite's texture atlas.
        /// </summary>
        public List<Rectangle> Frames { get; set; }

        public int FrameCount
        {
            get
            {
                return Frames.Count;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the animation</param>
        public AnimationData( string name )
        {
            Name = name;
            Frames = new List<Rectangle>();
        }
    }
}
