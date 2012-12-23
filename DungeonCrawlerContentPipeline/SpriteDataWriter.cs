using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Scott.Dungeon.Data;

// TODO: replace this with the type you want to write out.
using TWrite = Scott.Dungeon.Pipeline.SpriteDataContent;

namespace Scott.Dungeon.Pipeline
{
    /// <summary>
    /// Writes a sprite data out
    /// </summary>
    [ContentTypeWriter]
    public class SpriteDataWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write( ContentWriter output, TWrite value )
        {
            // Ensure the texture atlas is set
            if ( value.Texture == null )
            {
                throw new InvalidContentException( "Texture atlas is missing for " + value.Name );
            }

            // Make sure there's a valid default animation
            if ( value.DefaultAnimationName == null || !value.Animations.ContainsKey( value.DefaultAnimationName ) )
            {
                throw new InvalidContentException( "Sprite is missing a default animation, or it is incorrectly named" );
            }

            // Can't have too many animations in here
            if ( value.Animations.Count > 255 )
            {
                throw new InvalidContentException( "Cannot have more than 255 animations for sprite " + value.Name );
            }

            // Ensure there is at least one animation, and that every animation has at least one
            // frame of animation
            if ( value.Animations == null || value.Animations.Count < 1 )
            {
                throw new InvalidContentException( "Missing at least one animation for " + value.Name );
            }
            else
            {
                foreach ( KeyValuePair<string,AnimationData> keyValue in value.Animations )
                {
                    AnimationData animation = keyValue.Value;

                    if ( animation.Frames == null || animation.Frames.Count < 1 )
                    {
                        throw new InvalidContentException( "Missing at least one frame for sprite " + value.Name + ", animation " + animation.Name );
                    }

                    // Can't have too many frames in our animation
                    if ( animation.FrameCount > 255 )
                    {
                        throw new InvalidContentException( "Animation cannot have more than 255 frames for sprite " + value.Name + ", animation " + animation.Name );
                    }
                }
            }

            // Write a sprite header out
            output.Write( value.Name );
            output.WriteObject<TextureContent>( value.Texture );
            output.Write( (byte) value.Animations.Count );
            output.Write( value.DefaultAnimationName );

            // Now write the sprite's animations out
            foreach ( KeyValuePair<string,AnimationData> keyValue in value.Animations )
            {
                AnimationData animation = keyValue.Value;

                output.Write( animation.Name );
                output.Write( (byte) animation.Frames.Count );

                foreach ( Rectangle rect in keyValue.Value.Frames )
                {
                    output.Write( (ushort) rect.X );
                    output.Write( (ushort) rect.Y );
                    output.Write( (ushort) rect.Width );
                    output.Write( (ushort) rect.Height );
                }
            }
        }

        public override string GetRuntimeReader( TargetPlatform targetPlatform )
        {
            return "Scott.Dungeon.Data.SpriteDataReader, DungeonCrawlerCommon";
        }

        public override string GetRuntimeType( TargetPlatform targetPlatform )
        {
            return "Scott.Dungeon.Data.SpriteData, DungeonCrawlerCommon";
        }
    }
}
