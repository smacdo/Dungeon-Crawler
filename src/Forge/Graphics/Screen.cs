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
using System.Diagnostics;

namespace Forge.Graphics
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
