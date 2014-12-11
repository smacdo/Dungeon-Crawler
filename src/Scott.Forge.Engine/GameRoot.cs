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
        public static DebugRenderer Debug { get; private set; }
        public static Renderer Renderer { get; private set; }
        public static System.Random Random { get; private set; }

        /// <summary>
        /// THIS IS A HUGE HACK THAT WAS ONLY PUT HERE SO WE CAN GET A FINAL BUILD
        /// DONE TONIGHT.
        /// 
        /// DO NOT USE THIS AGAIN. Instead refactor the codebase where components are
        /// created by processors (aka factories). The player should then request the
        /// bounding box component factory, and query for any bounding boxes located in
        /// the requested area.
        /// </summary>
        public static List<GameObject> Enemies { get; private set; }

        private static GraphicsDevice mGraphicsDevice;

        /// <summary>
        /// Initialize the game root
        /// </summary>
        public static void Initialize( GraphicsDevice graphics, ContentManager content )
        {
            Enemies = new List<GameObject>();
            Debug = new DebugRenderer( graphics, content );
            mGraphicsDevice = graphics;
            Renderer = new Renderer( graphics );
            Random = new System.Random(); //  new Scott.Common.Random( Common.RandomGeneratorType.MersenneTwister );
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
        }
    }
}
