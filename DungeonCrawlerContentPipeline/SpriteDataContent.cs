using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scott.Dungeon.Data;

namespace Scott.Dungeon.Pipeline
{
    /// <summary>
    /// Build time representation for the SpriteData class
    /// </summary>
    public class SpriteDataContent
    {
        /// <summary>
        /// Name of the sprite
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The texture file path
        /// </summary>
        public string TextureFilePath { get; set; }

        /// <summary>
        /// The sprite's texture atlas
        /// </summary>
        public TextureContent Texture { get; set; }

        /// <summary>
        /// List of animations for this sprite
        /// </summary>
        public Dictionary<string, AnimationData> Animations { get; set; }

        /// <summary>
        /// Name of the default animation for this sprite
        /// </summary>
        public string DefaultAnimationName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="texture">Texture atlas that contains this sprite</param>
        public SpriteDataContent( string name, string texturePath )
        {
            Name = name;
            TextureFilePath = texturePath;
            Animations = new Dictionary<string, AnimationData>();
        }
    }
}
