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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scott.Forge.Engine.Graphics;
using Scott.Forge.GameObjects;

namespace Scott.Forge.Engine
{
    /// <summary>
    /// Global singleton that stores global systems, processors and the game object
    /// collection
    /// </summary>
    public static class GameRoot
    {
        public static IDebugOverlay Debug { get; internal set; }
        public static GameRenderer Renderer { get; private set; }
        public static System.Random Random { get; private set; }
        public static GameSettings Settings { get; private set; }
        public static bool UnitTests { get; private set; } = false;

        private static GraphicsDevice mGraphicsDevice;

        /// <summary>
        /// Initialize the game root
        /// </summary>
        public static void Initialize( GraphicsDevice graphics, ContentManager content )
        {
            mGraphicsDevice = graphics;

            if (graphics != null && content != null)
            {
                Debug = new StandardDebugOverlay(graphics, content);
                Renderer = new GameRenderer(graphics);
            }
            
            Random = new System.Random();
            Settings = new GameSettings();
        }

        /// <summary>
        /// Unloads loaded systems... call when the game is about to be unloaded
        /// </summary>
        public static void Unload()
        {
            Debug.Unload();
            mGraphicsDevice = null;
            Renderer = null;
            Debug = null;
            Settings = null;
        }
    }
}
