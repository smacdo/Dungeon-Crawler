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
using TInput = scott.dungeon.pipeline.SpriteFile;
using TOutput = scott.dungeon.SpriteData;

namespace scott.dungeon.pipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor( DisplayName = "Sprite Xml File Processor" )]
    public class SpriteContentProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process( TInput input, ContentProcessorContext context )
        {
            OpaqueDataDictionary textureParams = new OpaqueDataDictionary {
                { "ColorKeyEnabled", false },
                { "GenerateMipMaps", false },
                { "ResizeToPowerOfTwo", false },
                { "TextureFormat", TextureProcessorOutputFormat.Color }
            };

            ExternalReference<TextureContent> textureSource = new ExternalReference<TextureContent>( input.TexturePath );
            ExternalReference<TextureContent> textureRef = context.BuildAsset<TextureContent, TextureContent>( textureSource, "PremultipliedTextureProcessor", textureParams, null, null );
        }
    }
}