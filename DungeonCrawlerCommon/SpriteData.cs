using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scott.dungeon
{
    /// <summary>
    /// Information about a sprite and all of it's animations.
    /// </summary>
    public class SpriteData<T>
    {
        /// <summary>
        /// Name of the sprite
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sprite's texture atlas
        /// </summary>
        public T Texture { get; set; }

        /// <summary>
        /// List of animations for this sprite
        /// </summary>
        public Dictionary<string, SpriteAnimationData> Animations { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="texture">Texture atlas that contains this sprite</param>
        public SpriteData( string name, T texture )
        {
            Name = name;
            Texture = texture;
            Animations = new Dictionary<string, SpriteAnimationData>();
        }
    }
}
