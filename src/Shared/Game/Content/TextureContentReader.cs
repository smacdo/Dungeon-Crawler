using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scott.Game.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Scott.Game.Content
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
                                        string filePath,
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
