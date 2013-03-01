using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Scott.GameContent
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

        public AnimationData DefaultAnimation { get; private set; }

        /// <summary>
        /// The default animation name for this sprite
        /// </summary>
        public string DefaultAnimationName{ get; set; }

        /// <summary>
        /// Direction for the default animation
        /// </summary>
        public Direction DefaultAnimationDirection { get; set; }

        /// <summary>
        /// Offset from local origin when rendering
        /// </summary>
        public Vector2 OriginOffset { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="texture">Texture atlas that contains this sprite</param>
        /// <param name="defaultAnimation">Default animation name</param>
        /// <param name="defaultDirection">Default direction to use when animating without a direction</param>
        public SpriteData( string name,
                           Texture2D texture,
                           string defaultAnimation,
                           Direction defaultDirection,
                           List<AnimationData> animationList )
        {
            Name = name;
            Texture = texture;
            
            // Copy animations
            Animations = new Dictionary<string, AnimationData>( animationList.Count );

            foreach ( AnimationData animation in animationList )
            {
                Animations.Add( animation.Name, animation );
            }

            DefaultAnimationName = defaultAnimation;
            DefaultAnimationDirection = defaultDirection;
            DefaultAnimation = Animations[DefaultAnimationName];
            OriginOffset = Vector2.Zero;
        }
    }

    /// <summary>
    /// Loads sprite data from a content file
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
            Direction defDir   = (Direction) input.ReadByte();
            Vector2 offset     = input.ReadVector2();

            // Now read in all of the sprite's animations
            List<AnimationData> animations = new List<AnimationData>();

            for ( int animationIndex = 0; animationIndex < animationCount; ++animationIndex )
            {
                string animationName = input.ReadString();
                float frameTime      = input.ReadSingle();
                int frameCount       = (int) input.ReadByte();

                List<List<Rectangle>> allFrames = new List<List<Rectangle>>( Constants.DIRECTION_COUNT );

                // Read in each of the animation's four animatable directions
                for ( int dirIndex = 0; dirIndex < Constants.DIRECTION_COUNT; ++dirIndex )
                {
                    allFrames.Add( new List<Rectangle>( frameCount ) );

                    for ( int frameIndex = 0; frameIndex < frameCount; ++frameIndex )
                    {
                        int x = (int) input.ReadUInt16();
                        int y = (int) input.ReadUInt16();
                        int w = (int) input.ReadUInt16();
                        int h = (int) input.ReadUInt16();

                        allFrames[dirIndex].Add( new Rectangle( x, y, w, h ) );
                    }
                }

                // Construct the sprite animation data instance
                animations.Add( new AnimationData( animationName, frameTime, allFrames ) );
            }

            // Allocate a sprite data instance
            SpriteData spriteData   = new SpriteData( name, atlas, defAnimName, defDir, animations );
            spriteData.OriginOffset = offset;

            return spriteData;
        }
    }
}
