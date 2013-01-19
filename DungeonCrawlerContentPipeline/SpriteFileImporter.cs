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
            return ImportSpriteData( xml.SelectSingleNode( "/sprite" ), filename, context );
        }

        private SpriteDataContent ImportSpriteData( XmlNode spriteNode, string filename, ContentImporterContext context )
        {
            XmlNodeList animationNodes = spriteNode.SelectNodes( "animation" );

            // What is the name of this sprite?
            string spriteName = spriteNode.Attributes["name"].Value;

            // Get information on the texture atlas used for this sprite
            XmlNode atlasNode = spriteNode.SelectSingleNode( "atlas" );

            string imagePath = atlasNode.Attributes["image"].Value;
            int spriteWidth = Convert.ToInt32( atlasNode.Attributes["spriteWidth"].Value );
            int spriteHeight = Convert.ToInt32( atlasNode.Attributes["spriteHeight"].Value );

            // Get information on the default sprite name
            XmlNode defaultNode = spriteNode.SelectSingleNode( "default" );

            string defaultAnimationName = defaultNode.Attributes["animation"].Value;
            Direction defaultDirection  = (Direction) Enum.Parse( typeof( Direction ), defaultNode.Attributes["direction"].Value );

            // This sprite depends on it's texture atlas
            string fullImagePath = Path.Combine( Path.GetDirectoryName( filename ), imagePath );
            context.AddDependency( fullImagePath );

            // Generate a new sprite data object to store information from the imported XML file
            SpriteDataContent sprite = new SpriteDataContent( spriteName, fullImagePath );

            sprite.DefaultAnimationName = defaultAnimationName;
            sprite.DefaultAnimationDirection = defaultDirection;

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
                AnimationData animation = ImportAnimationData( animNode, spriteWidth, spriteHeight );
                sprite.Animations.Add( animation.Name, animation );
            }

            // All done, return the imported sprite
            return sprite;
        }

        private AnimationData ImportAnimationData( XmlNode animNode, int spriteWidth, int spriteHeight )
        {
            // Read animation header values
            string animationName  = animNode.Attributes["name"].Value;
            float frameTime       = Convert.ToSingle( animNode.Attributes["frameTime"].Value );

            // Read the animatable frame groups out. There should be either four (one for each
            // direction), or there should a single one named "*". Store these into a map for
            // easier retrieval
            Dictionary<Direction, XmlNodeList > frameGroupsTable = new Dictionary<Direction, XmlNodeList>();
            XmlNodeList frameGroups = animNode.SelectNodes( "frames" );

            foreach ( XmlNode groupNode in frameGroups )
            {
                string directionName = groupNode.Attributes["direction"].Value;

                if ( directionName == "*" )
                {
                    frameGroupsTable.Add( Direction.North, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( Direction.East, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( Direction.South, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( Direction.West, groupNode.SelectNodes( "frame" ) );
                }
                else
                {
                    Direction dir = (Direction) Enum.Parse( typeof( Direction ), directionName );
                    frameGroupsTable.Add( dir, groupNode.SelectNodes( "frame" ) );
                }
            }

            // Now create a list to store all of four of the animatable directions and shove each
            // group's rectangles in.
            List<List<Rectangle>> allFrames = new List<List<Rectangle>>( Constants.DIRECTION_COUNT );

            for ( int dirIndex = 0; dirIndex < Constants.DIRECTION_COUNT; ++dirIndex )
            {
                XmlNodeList frameNodes = frameGroupsTable[(Direction) dirIndex];
                List<Rectangle> frames = new List<Rectangle>();

                // Iterate though all the frames in the animation
                foreach ( XmlNode frameNode in frameNodes )
                {
                    int x = Convert.ToInt32( frameNode.Attributes["x"].Value );
                    int y = Convert.ToInt32( frameNode.Attributes["y"].Value );

                    frames.Add( new Rectangle( x, y, spriteWidth, spriteHeight ) );
                }

                allFrames.Add( frames );
            }

            // Add the animation to the sprite object
            return new AnimationData( animationName, frameTime, allFrames );
        }
    }
}
