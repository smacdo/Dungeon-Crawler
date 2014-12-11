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
using System;
using System.Diagnostics;
using System.IO;

namespace Scott.Forge.Engine.Content
{
    /// <summary>
    ///  Responsible for loading Texture2D instances from image input streams.
    /// </summary>
    [ContentReaderAttribute( typeof( Texture2D ), ".png" )]
    internal class TextureContentReader : ContentReader<Texture2D>
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
        ///  Construct a new SpriteData instance from disk.
        /// </summary>
        /// <returns>SpriteData instance.</returns>
        public override Texture2D Read( Stream input,
                                        string assetName,
                                        string contentDir,
                                        ContentManagerX content )
        {
            IServiceProvider provider = content.ServiceProvider;
            IGraphicsDeviceService iGraphics =
                (IGraphicsDeviceService) provider.GetService( typeof( IGraphicsDeviceService ) );
            GraphicsDevice graphics = iGraphics.GraphicsDevice;

            Debug.Assert( graphics != null, "Failed to retrieve active GraphicsDevice");

            return Texture2D.FromStream( graphics, input );
        }
    }
}
