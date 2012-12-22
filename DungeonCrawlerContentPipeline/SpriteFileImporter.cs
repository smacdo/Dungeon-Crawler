using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml;

// TODO: replace this with the type you want to import.
using TImport = scott.dungeon.pipeline.SpriteFile;

namespace scott.dungeon.pipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter( ".sprite.xml", DisplayName = "Sprite Xml Importer", DefaultProcessor = "SpriteContentProcessor" )]
    public class SpriteFileImporter : ContentImporter<TImport>
    {
        public override TImport Import( string filename, ContentImporterContext context )
        {
            XmlDocument xml = new XmlDocument();
            xml.Load( filename );

            XmlNode spriteNode = xml.SelectSingleNode( "/SpriteContent/Sprite" );

            string spriteName = spriteNode.Attributes["name"].Value;
            string imagePath = spriteNode.Attributes["image"].Value;
            int spriteWidth = Convert.ToInt32( spriteNode.Attributes["spriteWidth"] );
            int spriteHeight = Convert.ToInt32( spriteNode.Attributes["spriteHeight"] );

            context.AddDependency( imagePath );

            SpriteFile spriteFile = new SpriteFile( spriteName, textureName );

            XmlNodeList animationNodes = spriteNode.SelectNodes( "animation" );

            foreach ( XmlNode animNode in animationNodes )
            {
                string animationName =  animNode.Attributes["name"].Value 
                SpriteFile.AnimationInfo animation = new SpriteFile.AnimationInfo( animationName );

                XmlNodeList frameNodes = animNode.SelectNodes( "frame" );

                foreach ( XmlNode frameNode in frameNodes )
                {
                    int x = Convert.ToInt32( frameNode.Attributes["x"].Value );
                    int y = Convert.ToInt32( frameNode.Attributes["y"].Value );

                    animation.FrameOffsets.Add( new Rectangle( x, y, spriteWidth, spriteHeight ) );
                }

                spriteFile.Animations.Add( animation );
            }

            return spriteFile;
        }
    }
}
