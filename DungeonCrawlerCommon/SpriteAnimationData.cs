using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace scott.dungeon
{
    /// <summary>
    /// Information about a sprite animation
    /// </summary>
    public class SpriteAnimationData
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the animation</param>
        public SpriteAnimationData( string name )
        {
            Name = name;
            Frames = new List<Rectangle>();
        }
    }
}
