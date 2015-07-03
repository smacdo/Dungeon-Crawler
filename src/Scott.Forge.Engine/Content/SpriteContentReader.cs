/*
 * Copyright 2012-2014 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Forge.Engine.Graphics;
using Scott.Forge.Engine.Sprites;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Responsible for loading SpriteData instances from an input stream.
    /// </summary>
    [ContentReaderAttribute( typeof( SpriteDefinition ), ".sprite" )]
    internal class SpriteContentReader : ContentReader<SpriteDefinition>
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
        public override SpriteDefinition Read( Stream input,
                                         string assetName,
                                         string contentDir,
                                         ContentManagerX content )
        {
            XmlDocument xml  = new XmlDocument();
            xml.Load( input );

            return  ImportSpriteData( xml.SelectSingleNode( "/sprite" ), contentDir, content );
        }

        /// <summary>
        ///  Load SpriteData from an XML document.
        /// </summary>
        /// <param name="spriteNode"></param>
        /// <param name="contentDir"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private SpriteDefinition ImportSpriteData( XmlNode spriteNode, string contentDir, ContentManagerX content )
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
            var defaultDirection  = DirectionNameHelper.Parse(defaultNode.Attributes["direction"].Value );

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
            List<AnimationDefinition> animations = new List<AnimationDefinition>( animationNodes.Count );

            foreach ( XmlNode animNode in animationNodes )
            {
                AnimationDefinition animation = ImportAnimationData( animNode, spriteWidth, spriteHeight );
                animations.Add( animation );
            }

            // All done, return the imported sprite
            return new SpriteDefinition( spriteName, atlas, defaultAnimation, defaultDirection, animations );
        }

        private AnimationDefinition ImportAnimationData( XmlNode animNode, int spriteWidth, int spriteHeight )
        {
            // Read animation header values
            string animationName  = animNode.Attributes["name"].Value;
            float frameTime       = Convert.ToSingle( animNode.Attributes["frameTime"].Value );

            // Read the animatable frame groups out. There should be either four (one for each
            // direction), or there should a single one named "*". Store these into a map for
            // easier retrieval
            var frameGroupsTable = new Dictionary<DirectionName, XmlNodeList>();
            XmlNodeList frameGroups = animNode.SelectNodes( "frames" );

            foreach ( XmlNode groupNode in frameGroups )
            {
                string directionName = groupNode.Attributes["direction"].Value;

                if ( directionName == "*" )
                {
                    frameGroupsTable.Add( DirectionName.North, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( DirectionName.East, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( DirectionName.South, groupNode.SelectNodes( "frame" ) );
                    frameGroupsTable.Add( DirectionName.West, groupNode.SelectNodes( "frame" ) );
                }
                else
                {
                    var dir = DirectionNameHelper.Parse(directionName);
                    frameGroupsTable.Add( dir, groupNode.SelectNodes( "frame" ) );
                }
            }

            // Now create a list to store all of four of the animatable directions and shove each
            // group's rectangles in.
            var allFrames = new List<List<RectF>>( Constants.DirectionCount );

            for (int dirIndex = 0; dirIndex < Constants.DirectionCount; ++dirIndex)
            {
                XmlNodeList frameNodes = frameGroupsTable[(DirectionName) dirIndex];    // TODO: Remove cast?
                var frames = new List<RectF>();

                // Iterate though all the frames in the animation
                foreach ( XmlNode frameNode in frameNodes )
                {
                    int x = Convert.ToInt32( frameNode.Attributes["x"].Value );
                    int y = Convert.ToInt32( frameNode.Attributes["y"].Value );

                    frames.Add( new RectF( x, y, spriteWidth, spriteHeight ) );
                }

                allFrames.Add( frames );
            }

            // Add the animation to the sprite object
            return new AnimationDefinition( animationName, frameTime, allFrames );
        }
    }
}
