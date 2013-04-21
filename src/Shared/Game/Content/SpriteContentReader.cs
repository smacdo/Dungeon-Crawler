using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Game.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Scott.Game.Content
{
    /// <summary>
    ///  Responsible for loading SpriteData instances from an input stream.
    /// </summary>
    [ContentReaderAttribute( typeof( SpriteData ), ".sprite" )]
    internal class SpriteContentReader : ContentReader<SpriteData>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public SpriteContentReader()
            : base()
        {
            // Empty
        }

        /// <summary>
        ///  Construct a new SpriteData instance from disk.
        /// </summary>
        /// <returns>SpriteData instance.</returns>
        public override SpriteData Read( Stream input,
                                         string assetName,
                                         string contentDir,
                                         string filePath,
                                         ContentManagerX content )
        {
            SpriteData sprite = null;

 //           try
            {
                // SpriteData is stored as an XML document, so create a file stream and convert it
                // into an XML document.
//                FileStream input = new FileStream( mFilename, FileMode.Open );
                XmlDocument xml  = new XmlDocument();
                xml.Load( input );

                sprite = ImportSpriteData( xml.SelectSingleNode( "/sprite" ), contentDir, content );
            }
/*            catch ( System.Exception ex )
            {
                throw new GameContentException( "Exception while reading game content",
                                                filePath,
                                                ex );
            }*/

            return sprite;
        }

        /// <summary>
        ///  Load SpriteData from an XML document.
        /// </summary>
        /// <param name="spriteNode"></param>
        /// <param name="contentDir"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private SpriteData ImportSpriteData( XmlNode spriteNode, string contentDir, ContentManagerX content )
        {
            XmlNodeList animationNodes = spriteNode.SelectNodes( "animation" );

            // What is the name of this sprite?
            string spriteName = spriteNode.Attributes["name"].Value;

            // Get information on the texture atlas used for this sprite
            XmlNode atlasNode = spriteNode.SelectSingleNode( "atlas" );

            string atlasName = atlasNode.Attributes["ref"].Value;
            int spriteWidth = Convert.ToInt32( atlasNode.Attributes["spriteWidth"].Value );
            int spriteHeight = Convert.ToInt32( atlasNode.Attributes["spriteHeight"].Value );

            // Get information on the default sprite name
            XmlNode defaultNode = spriteNode.SelectSingleNode( "default" );

            string defaultAnimation = defaultNode.Attributes["animation"].Value;
            Direction defaultDirection  = (Direction) Enum.Parse( typeof( Direction ), defaultNode.Attributes["direction"].Value );

            // This sprite depends on it's texture atlas
            Texture2D atlas = content.Load<Texture2D>( atlasName ); 

            // Does the sprite have an offset?
            Vector2 offset = Vector2.Zero;

            if ( spriteNode.Attributes["offsetX"] != null && spriteNode.Attributes["offsetY"] != null )
            {
                int offsetX = Convert.ToInt32( spriteNode.Attributes["offsetX"].Value );
                int offsetY = Convert.ToInt32( spriteNode.Attributes["offsetY"].Value );

                offset = new Vector2( offsetX, offsetY );
            }

            // Iterate through all the animation nodes, and process them
            List<AnimationData> animations = new List<AnimationData>( animationNodes.Count );

            foreach ( XmlNode animNode in animationNodes )
            {
                AnimationData animation = ImportAnimationData( animNode, spriteWidth, spriteHeight );
                animations.Add( animation );
            }

            // All done, return the imported sprite
            return new SpriteData( spriteName, atlas, defaultAnimation, defaultDirection, animations );
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
