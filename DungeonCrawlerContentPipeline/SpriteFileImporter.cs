using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml;
using System.IO;
using Scott.Dungeon.Data;

// TODO: replace this with the type you want to import.
using TImport = Scott.Dungeon.Pipeline.SpriteDataContent;

namespace Scott.Dungeon.Pipeline
{
    /// <summary>
    /// Imports a sprite XML file
    /// </summary>
    [ContentImporter( ".sprite", DisplayName = "Sprite Xml Importer", DefaultProcessor = "SpriteContentProcessor" )]
    public class SpriteFileImporter : ContentImporter<TImport>
    {
        public override TImport Import( string filename, ContentImporterContext context )
        {
            // Load the XML file
            XmlDocument xml = new XmlDocument();
            xml.Load( filename );

            // Grab the root <sprite> node, and then a list of it's animation XML nodes
            XmlNode spriteNode = xml.SelectSingleNode( "/sprite" );
            XmlNodeList animationNodes = spriteNode.SelectNodes( "animation" );

            // Parse out the sprite's base attributes
            string spriteName = spriteNode.Attributes["name"].Value;
            string imagePath = spriteNode.Attributes["image"].Value;
            int spriteWidth = Convert.ToInt32( spriteNode.Attributes["spriteWidth"].Value );
            int spriteHeight = Convert.ToInt32( spriteNode.Attributes["spriteHeight"].Value );

            // This sprite depends on it's texture atlas
            string fullImagePath = Path.Combine( Path.GetDirectoryName( filename ), imagePath );
            context.AddDependency( fullImagePath );

            // Generate a new sprite data object to store information from the imported XML file
            SpriteDataContent sprite = new SpriteDataContent( spriteName, fullImagePath );

            // Does the sprite have an offset?
            if ( spriteNode.Attributes["offsetX"] != null && spriteNode.Attributes["offsetY"] != null )
            {
                int offsetX = Convert.ToInt32( spriteNode.Attributes["offsetX"].Value );
                int offsetY = Convert.ToInt32( spriteNode.Attributes["offsetY"].Value );

                sprite.OriginOffset = new Vector2( offsetX, offsetY );
            }

            // Iterate through all the animation nodes, and process them
            foreach ( XmlNode animNode in animationNodes )
            {
                string animationName    = animNode.Attributes["name"].Value;
                AnimationData animation = new AnimationData( animationName );

                // Iterate though all the frames in the animation
                XmlNodeList frameNodes = animNode.SelectNodes( "frame" );
                
                foreach ( XmlNode frameNode in frameNodes )
                {
                    int x = Convert.ToInt32( frameNode.Attributes["x"].Value );
                    int y = Convert.ToInt32( frameNode.Attributes["y"].Value );

                    animation.Frames.Add( new Rectangle( x, y, spriteWidth, spriteHeight ) );
                }

                // Is this the default animation?
                bool hasDefaultAttribute = ( animNode.Attributes["default"] != null );
                
                if ( hasDefaultAttribute && Convert.ToBoolean( animNode.Attributes["default"].Value ) )
                {
                    sprite.DefaultAnimationName = animationName;
                }
               
                // Add the animation to the sprite object
                sprite.Animations.Add( animationName, animation );
            }

            // All done, return the imported sprite
            return sprite;
        }
    }
}
