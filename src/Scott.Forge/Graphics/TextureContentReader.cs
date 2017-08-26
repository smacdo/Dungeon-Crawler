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
using Microsoft.Xna.Framework.Graphics;
using Scott.Forge.Content;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Scott.Forge.Graphics
{
    /// <summary>
    ///  Responsible for loading Texture2D instances from image input streams.
    /// </summary>
    [ContentReader(typeof(Texture2D), ".png")]
    internal class TextureContentReader : IContentReader<Texture2D>
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public TextureContentReader()
            : base()
        {
            // Empty
        }

        /// <summary>
        ///  Read a serialized asset from an input stream and return it as a loaded object.
        /// </summary>
        /// <param name="inputStream">Stream to read serialized asset data from.</param>
        /// <param name="assetPath">Relative path to the serialized asset.</param>
        /// <param name="content">Content manager.</param>
        /// <returns>Deserialized content object.</returns>
        public Task<Texture2D> Read(
            Stream inputStream,
            string assetPath,
            IContentManager content)
        {
            var graphicsDeviceService =
                (IGraphicsDeviceService)content.XnaContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));

            if (graphicsDeviceService.GraphicsDevice == null)
            {
                throw new InvalidOperationException("Graphics device service not registered");
            }
            
            // TODO: FromStream is (probably) not async ready. Read the texture into a memory buffer that is
            // async awaitable, and then pass that stream to FromStream. Also use using to try to immediately
            // release the buffer from memory since it takes up a lot of space.
            return Task.FromResult(Texture2D.FromStream(graphicsDeviceService.GraphicsDevice, inputStream));
        }
    }
}
