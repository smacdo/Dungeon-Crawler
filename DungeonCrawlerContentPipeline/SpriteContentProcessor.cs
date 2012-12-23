using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.IO;

// TODO: replace these with the processor input and output types.
using TInput = Scott.Dungeon.Pipeline.SpriteDataContent;
using TOutput = Scott.Dungeon.Pipeline.SpriteDataContent;

namespace Scott.Dungeon.Pipeline
{
    /// <summary>
    /// Processes an imported sprite data by transforming it's atlas file path into a loaded external
    /// reference
    /// </summary>
    [ContentProcessor( DisplayName = "Sprite Xml File Processor" )]
    public class SpriteContentProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process( TInput input, ContentProcessorContext context )
        {


            ExternalReference<TextureContent> atlasReference = new ExternalReference<TextureContent>( input.TextureFilePath );
            input.Texture = context.BuildAndLoadAsset<TextureContent, TextureContent>( atlasReference, "SpriteTextureProcessor" );

            // All done
            return input;

            /*  deprecated, keeping it here until we're sure we don't need to change texture load params
            OpaqueDataDictionary textureParams = new OpaqueDataDictionary {
                { "ColorKeyEnabled", false },
                { "GenerateMipMaps", false },
                { "ResizeToPowerOfTwo", false },
                { "TextureFormat", TextureProcessorOutputFormat.Color }
            };

            ExternalReference<TextureContent> textureSource = new ExternalReference<TextureContent>( input.TexturePath );
            ExternalReference<TextureContent> textureRef = context.BuildAsset<TextureContent, TextureContent>( textureSource, "PremultipliedTextureProcessor", textureParams, null, null );
            */
        }
    }
}