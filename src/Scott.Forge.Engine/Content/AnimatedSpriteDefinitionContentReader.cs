/*
 * Copyright 2012-2017 Scott MacDonald.
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
    ///  TODO: Make this more robust with errors when unexpected values are encountered.
    ///  TODO: Convert file format to JSON.
    /// </summary>
    [ContentReaderAttribute( typeof( AnimatedSpriteDefinition ), ".sprite" )]
    internal class AnimatedSpriteDefinitionContentReader : ContentReader<AnimatedSpriteDefinition>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public AnimatedSpriteDefinitionContentReader()
            : base()
        {
            // Empty
        }

        /// <summary>
        ///  Construct a new SpriteData instance from disk.
        /// </summary>
        /// <returns>SpriteData instance.</returns>
        public override AnimatedSpriteDefinition Read( Stream input,
                                         string assetName,
                                         string contentDir,
                                         ForgeContentManager content )
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
        private AnimatedSpriteDefinition ImportSpriteData(XmlNode spriteNode, string contentDir, ForgeContentManager content)
        {
            return new AnimatedSpriteDefinition(
                ImportSpriteDefinition(spriteNode, content),
                ImportAnimationSetDefintion(spriteNode));
        }
        
        private SpriteDefinition ImportSpriteDefinition(XmlNode rootNode, ForgeContentManager content)
        {
            // What is the name of this sprite?
            string spriteName = rootNode.Attributes["name"].Value;

            // Get information on the texture atlas used for this sprite
            var atlasNode = rootNode.SelectSingleNode( "atlas" );

            string atlasName = atlasNode.Attributes["ref"].Value;
            int spriteWidth = Convert.ToInt32( atlasNode.Attributes["spriteWidth"].Value );
            int spriteHeight = Convert.ToInt32( atlasNode.Attributes["spriteHeight"].Value );

            // Find the sprite atlas x/y offset.
            // TODO: Currently using the animation data to get this. Stop using this hacky solution and directly encode
            //       the initial x/y offset. Probably should do this when we convert the data from XML to JSON.
            var startingOffset = Vector2.Zero;

            var defaultNode = rootNode.SelectSingleNode("default");
            string defaultAnimationName = defaultNode.Attributes["animation"].Value;

            var animationNodes = rootNode.SelectNodes( "animation" );
            var animations = new List<AnimationDefinition>(animationNodes.Count);

            foreach (XmlNode animNode in animationNodes)
            {
                var animationName = animNode.Attributes["name"].Value;

                if (animationName == defaultAnimationName)
                {
                    var animation = ImportAnimationData(animNode);
                    startingOffset = animation.GetAtlasPosition(Constants.DefaultDirection, 0);
                }
            }

            // Grab the texture atlas.
            var atlas = content.Load<Texture2D>(atlasName);

            // All done.
            return new SpriteDefinition(spriteName, new SizeF(spriteWidth, spriteHeight), startingOffset, atlas);
        }

        private AnimationSetDefinition ImportAnimationSetDefintion(XmlNode rootNode)
        {
            // Iterate through all the animation nodes, and process them.
            var animationNodes = rootNode.SelectNodes("animation");
            var animations = new List<AnimationDefinition>(animationNodes.Count);

            foreach (XmlNode animNode in animationNodes)
            {
                var animation = ImportAnimationData(animNode);
                animations.Add(animation);
            }

            return new AnimationSetDefinition(animations);
        }

        private AnimationDefinition ImportAnimationData(XmlNode animNode)
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
            var allFrames = new List<List<Vector2>>( Constants.DirectionCount );

            for (int dirIndex = 0; dirIndex < Constants.DirectionCount; ++dirIndex)
            {
                XmlNodeList frameNodes = frameGroupsTable[(DirectionName) dirIndex];    // TODO: Remove cast?
                var frames = new List<Vector2>();

                // Iterate though all the frames in the animation
                foreach ( XmlNode frameNode in frameNodes )
                {
                    int x = Convert.ToInt32( frameNode.Attributes["x"].Value );
                    int y = Convert.ToInt32( frameNode.Attributes["y"].Value );

                    frames.Add(new Vector2(x, y));
                }

                allFrames.Add( frames );
            }

            // Add the animation to the sprite object
            return new AnimationDefinition( animationName, frameTime, allFrames );
        }
    }
}
