using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using scott.dungeon;

// TODO: replace this with the type you want to write out.
using TWrite = scott.dungeon.pipeline.SpriteDataContent;

namespace scott.dungeon.pipeline
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
                }
            }

            // Write a sprite header out
            output.Write( value.Name );
            output.WriteExternalReference( value.Texture );
            output.Write( value.Animations.Count );

            // Now write the sprite's animations out
            foreach ( KeyValuePair<string,AnimationData> keyValue in value.Animations )
            {
                AnimationData animation = keyValue.Value;

                output.Write( animation.Name );
                output.Write( animation.Frames.Count );

                foreach ( Rectangle rect in keyValue.Value.Frames )
                {
                    output.Write( rect.X );
                    output.Write( rect.Y );
                    output.Write( rect.Width );
                    output.Write( rect.Height );
                }
            }
        }

        public override string GetRuntimeReader( TargetPlatform targetPlatform )
        {
            return "scott.dungeon.SpriteDataReader, DungeonCrawlerCommon";
        }

        public override string GetRuntimeType( TargetPlatform targetPlatform )
        {
            return "scott.dungeon.SpriteData, DungeonCrawlerCommon";
        }
    }
}
