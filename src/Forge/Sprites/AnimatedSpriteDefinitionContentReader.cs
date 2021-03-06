﻿/*
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
using Forge.Content;
using System.Threading.Tasks;

namespace Forge.Sprites
{
    /// <summary>
    ///  Loads an animated sprite from an XML input stream and returns an AnimatedSpriteDefinition object.
    /// </summary>
    [ContentReader(typeof(AnimatedSpriteDefinition), ".sprite")]
    public class AnimatedSpriteDefinitionContentReader : IContentReader<AnimatedSpriteDefinition>
    {
        private string _assetPath;

        /// <summary>
        ///  Read an animated sprite from XML and convert it into an AnimatedSpriteDefinition object.
        /// </summary>
        /// <param name="inputStream">Stream to read serialized asset data from.</param>
        /// <param name="assetPath">Relative path to the serialized asset.</param>
        /// <param name="content">Content manager.</param>
        /// <returns>Deserialized content object.</returns>
        public async Task<AnimatedSpriteDefinition> Read(
            Stream inputStream,
            string assetPath,
            IContentManager content)
        {
            // Check arguments for validty.
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                throw new ArgumentNullException(nameof(assetPath));
            }

            _assetPath = assetPath;

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            // Convert input stream into an XML document.
            var xml  = new XmlDocument();

            try
            {
                xml.Load(inputStream);
            }
            catch (Exception e)
            {
                throw new ContentLoadException(
                    "Error when reading sprite animation definition. Is this a valid XML document?",
                    _assetPath,
                    e);
            }

            // Read and convert the XML document into an animation definition.
            return await ImportSpriteData(xml.SelectSingleNode("/sprite"), content);
        }

        /// <summary>
        ///  Reads an animated sprite definition element and convert it into a AnimatedSpriteDefinition object.
        /// </summary>
        /// <remarks>
        ///  Forge separates sprite definitions from animation definitions, but the legacy sprite format combines both
        ///  data types in the Sprite element. This method will read the element twice, extract the sprite definition
        ///  and animation definition before returning a AnimatedSpriteDefinition that holds both extracted objects.
        /// </remarks>
        /// <param name="spriteNode">Sprite xml element.</param>
        /// <param name="content">Forge content manager for loading referenced assets.</param>
        /// <returns>Parsed AnimatedSpriteDefinition object.</returns>
        private async Task<AnimatedSpriteDefinition> ImportSpriteData(
            XmlNode spriteNode,
            IContentManager content)
        {
            return new AnimatedSpriteDefinition(
                await ImportSpriteDefinition(spriteNode, content),
                ImportAnimationSetDefintion(spriteNode));
        }
        
        /// <summary>
        ///  Read a XML sprite element and convert it into a SpriteDefinition object.
        /// </summary>
        /// <remarks>
        ///  The legacy sprite format combines sprite and animation data. This method will extract only sprite data
        ///  from the sprite element, and ignore any animation data that is present.
        /// </remarks>
        /// <param name="spriteNode">Sprite xml element.</param>
        /// <param name="content">Forge content manager for loading referenced assets.</param>
        /// <returns>Parsed SpriteDefinition object.</returns>
        private async Task<SpriteDefinition> ImportSpriteDefinition(XmlNode spriteNode, IContentManager content)
        {
            // Get the name of the sprite. For files with only one sprite the name is the stream name, otherwise the
            // name should be specified as an attribute in the name.
            var spriteName = spriteNode.Attributes["name"]?.Value ?? _assetPath;

            if (string.IsNullOrWhiteSpace(spriteName))
            {
                throw new ContentLoadException("Sprite name missing or invalid", _assetPath);
            }

            // Get the sprite width and height.
            int spriteWidth = 0;
            int spriteHeight = 0;

            try
            {
                spriteWidth = Convert.ToInt32(spriteNode.Attributes["width"]?.Value);
                spriteHeight = Convert.ToInt32(spriteNode.Attributes["height"]?.Value);
            }
            catch (FormatException e)
            {
                throw new ContentLoadException("Sprite width or height missing or invalid", _assetPath, e);
            }

            // Get information on the texture atlas used for this sprite.
            var atlasNode = spriteNode.SelectSingleNode("atlas");

            if (atlasNode == null)
            {
                throw new ContentLoadException("Texture atlas element missing", _assetPath);
            }

            var atlasPath = atlasNode.Attributes["ref"]?.Value;

            if (string.IsNullOrWhiteSpace(atlasPath))
            {
                throw new ContentLoadException("Texture atlas path missing or invalid", _assetPath);
            }
            
            // Find the sprite atlas x/y offset.
            // TODO: Once the graphics renderer is rewritten to use Transform to get direction (Rather than being a part of the animation
            // being played) remove the reading of the default animation direction field.
            var atlasPosition = Vector2.Zero;

            try
            {
                atlasPosition = new Vector2(
                    Convert.ToInt32(atlasNode.Attributes["x"]?.Value),
                    Convert.ToInt32(atlasNode.Attributes["y"]?.Value));
            }
            catch (FormatException e)
            {
                throw new ContentLoadException("Sprite width or height missing or invalid", _assetPath, e);
            }

            // Grab the texture atlas.
            var atlas = await content.Load<Texture2D>(atlasPath);

            // All done.
            return new SpriteDefinition(
                spriteName,
                new SizeF(spriteWidth, spriteHeight),
                atlasPosition,
                atlas);
        }

        /// <summary>
        ///  Read a XML sprite element and convert it into a AnimationDefinition object.
        /// </summary>
        /// <remarks>
        ///  The legacy sprite format combines sprite and animation data. This method will extract only animation data
        ///  from the sprite element, and ignore any sprite data that is present.
        /// </remarks>
        /// <param name="spriteNode">Sprite xml element.</param>
        /// <returns>Parsed AnimationSetDefinition object.</returns>
        private AnimationSetDefinition ImportAnimationSetDefintion(XmlNode spriteNode)
        {
            // Iterate through all the animation nodes, and process them.
            var animationNodes = spriteNode.SelectNodes("animation");
            var animations = new List<AnimationDefinition>(animationNodes.Count);

            foreach (XmlNode animNode in animationNodes)
            {
                var animation = ImportAnimationData(animNode);
                animations.Add(animation);
            }

            return new AnimationSetDefinition(animations);
        }

        /// <summary>
        ///  Read an animation definition from a XML animation element.
        /// </summary>
        /// <param name="animationNode">Xml element holding one or more frame group elements.</param>
        /// <returns>Parsed animation definition.</returns>
        private AnimationDefinition ImportAnimationData(XmlNode animationNode)
        {
            // Get the animation name.
            var animationName = animationNode.Attributes["name"]?.Value;

            if (string.IsNullOrWhiteSpace(animationName))
            {
                throw new ContentLoadException("Animation name missing or invalid", _assetPath);
            }

            // Get amount of time each frame should be displayed for.
            var frameTime = AnimationDefinition.DefaultFrameTimeInSeconds;

            try
            {
                frameTime = Convert.ToSingle(animationNode.Attributes["frameTime"].Value);
            }
            catch (FormatException e)
            {
                throw new ContentLoadException("Invalid frame animation time", _assetPath, e);
            }
            
            // There needs to be at least one frameset specified.
            var frameGroups = animationNode.SelectNodes("frames");
            int frameGroupCount = frameGroups.Count;

            if (frameGroupCount < 1)
            {
                throw new ContentLoadException("Animation must have at least one frame group", _assetPath);
            }

            // Count the number of frames in the animation. Each direction must have the same number of frames so
            // tally up the node count in the first direction listed.
            var frameCount = frameGroups[0].SelectNodes("frame").Count;

            if (frameCount < 1)
            {
                throw new ContentLoadException("Animation must have at least one frame", _assetPath);
            }

            // Read the animation frame groups and write them into an multi-dimensional array that holds frames for
            // each animation direction.
            var frames = new SpriteFrame[frameGroupCount, frameCount];

            foreach (XmlNode groupNode in frameGroups)
            {
                var directionName = groupNode.Attributes["direction"]?.Value;
                var frameIndex = 0;

                if (directionName == null || directionName == "*")
                {
                    foreach (XmlNode frameNode in groupNode.SelectNodes("frame"))
                    {
                        frames[0, frameIndex++] = ReadAnimationFrame(frameNode);
                    }
                }
                else
                {
                    var direction = DirectionNameHelper.Parse(directionName);

                    foreach (XmlNode frameNode in groupNode.SelectNodes("frame"))
                    {
                        frames[(int)direction, frameIndex++] = ReadAnimationFrame(frameNode);
                    }
                }
            }

            // Add the animation to the sprite object
            return new AnimationDefinition( animationName, frameTime, frames );
        }

        /// <summary>
        ///  Read a frame element.
        /// </summary>
        /// <param name="frameNode">XML element representing the animation frame.</param>
        /// <returns>Frame data.</returns>
        private SpriteFrame ReadAnimationFrame(XmlNode frameNode)
        {
            try
            {
                // Get atlas top left position.
                var x = Convert.ToInt32(frameNode.Attributes["x"].Value);
                var y = Convert.ToInt32(frameNode.Attributes["y"].Value);

                // Get events associated with this frame.
                AnimationEvent[] events = null;
                var eventNodes = frameNode.SelectNodes("event");

                if (eventNodes.Count > 0)
                {
                    events = new AnimationEvent[eventNodes.Count];
                }
                
                for (int i = 0; i < eventNodes.Count; i++)
                {
                    events[i] = ReadAnimationFrameEvent(eventNodes[i]);
                }

                return new SpriteFrame()
                {
                    AtlasPosition = new Vector2(x, y),
                    Events = events
                };
            }
            catch (FormatException e)
            {
                throw new ContentLoadException(
                    "Invalid x or y value for animation frame",
                    _assetPath,
                    e);
            }
        }

        /// <summary>
        ///  Read an event element inside of a frame.
        /// </summary>
        /// <param name="eventNode">XML element representing the event.</param>
        /// <returns>Frame data.</returns>
        private AnimationEvent ReadAnimationFrameEvent(XmlNode eventNode)
        {
            // Read the event name.
            var eventName = eventNode.Attributes["name"].Value;
            
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ContentLoadException("Animation event name missing or invalid", _assetPath);
            }

            // Check for presence of arguments.
            Dictionary<string, string> args = null;
            var argNodes = eventNode.SelectNodes("arg");

            if (argNodes.Count > 0)
            {
                args = new Dictionary<string, string>(argNodes.Count);
            }

            foreach (XmlNode argNode in argNodes)
            {
                var argName = argNode.Attributes["name"].Value;
                var argValue = argNode.InnerText;

                if (string.IsNullOrEmpty(argName))
                {
                    throw new ContentLoadException("Animation event arg name missing or invalid", _assetPath);
                }

                args[argName] = argValue ?? throw new ContentLoadException(
                    "Animation event arg value missing or invalid", _assetPath);
            }

            // Return event.
            return new AnimationEvent(eventName, args);
        }
    }
}
