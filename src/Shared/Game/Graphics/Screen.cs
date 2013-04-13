using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Scott.Game.Graphics
{
    /// <summary>
    ///  Contains information on the main game window screen.
    /// </summary>
    public class Screen
    {
        private static GraphicsDevice mDevice;

        /// <summary>
        ///  Initialize the screen information.
        /// </summary>
        /// <param name="device"></param>
        public static void Initialize( GraphicsDevice device )
        {
            Debug.Assert( device != null );
            mDevice = device;
        }

        /// <summary>
        ///  Width of the game screen.
        /// </summary>
        public static int Width
        {
            get
            {
                return mDevice.Viewport.Width;
            }
        }

        /// <summary>
        ///  Height of the game screen.
        /// </summary>
        public static int Height
        {
            get
            {
                return mDevice.Viewport.Height;
            }
        }

        /// <summary>
        ///  Aspect ratio of the game screen.
        /// </summary>
        public static float AspectRatio
        {
            get
            {
                return mDevice.Viewport.AspectRatio;
            }
        }
    }
}
