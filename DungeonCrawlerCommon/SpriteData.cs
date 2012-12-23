using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Scott.Dungeon.Data
{
    /// <summary>
    /// Static information about a sprite and how to play it's animations. 
    /// </summary>
    public class SpriteData
    {
        /// <summary>
        /// Name of the sprite
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The sprite's texture atlas
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// List of animations for this sprite
        /// </summary>
        public Dictionary<string, AnimationData> Animations { get; set; }

        /// <summary>
        /// Name of the default animation for this sprite
        /// </summary>
        public string DefaultAnimationName { get; set; }

        /// <summary>
        /// Offset from local origin when rendering
        /// </summary>
        public Vector2 OriginOffset { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="texture">Texture atlas that contains this sprite</param>
        public SpriteData( string name, Texture2D texture )
        {
            Name = name;
            Texture = texture;
            Animations = new Dictionary<string, AnimationData>();
            OriginOffset = Vector2.Zero;
        }
    }

    /// <summary>
    /// Loads a sprite data
    /// </summary>
    public class SpriteDataReader : ContentTypeReader<SpriteData>
    {
        protected override SpriteData Read( ContentReader input, SpriteData existingInstance )
        {
            // Read the sprite header
            string name        = input.ReadString();
            Texture2D atlas    = input.ReadObject<Texture2D>();
            int animationCount = (int) input.ReadByte();
            string defAnimName = input.ReadString();
            Vector2 offset     = input.ReadVector2();
 
            // Allocate a sprite data instance
            SpriteData spriteData   = new SpriteData( name, atlas );

            // Now read in all of the sprite's animations
            for ( int animationIndex = 0; animationIndex < animationCount; ++animationIndex )
            {
                string animationName    = input.ReadString();
                int frameCount          = (int) input.ReadByte();
                AnimationData animation = new AnimationData( animationName );

                for ( int frameIndex = 0; frameIndex < frameCount; ++frameIndex )
                {
                    int x = (int) input.ReadUInt16();
                    int y = (int) input.ReadUInt16();
                    int w = (int) input.ReadUInt16();
                    int h = (int) input.ReadUInt16();

                    animation.Frames.Add( new Rectangle( x, y, w, h ) );
                }

                spriteData.Animations.Add( animationName, animation );
            }

            spriteData.OriginOffset = offset;
            spriteData.DefaultAnimationName = defAnimName;
            return spriteData;
        }
    }
}
