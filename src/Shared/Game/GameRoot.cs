using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scott.Game.Entity;
using Scott.Game.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Game
{
    /// <summary>
    /// Global singleton that stores global systems, processors and the game object
    /// collection
    /// </summary>
    public static class GameRoot
    {
        public static DebugRenderer Debug { get; private set; }
        public static Renderer Renderer { get; private set; }
        public static Random Random { get; private set; }

        /// <summary>
        /// Width (in pixels) of the camera's visible area
        /// </summary>
        public static int ViewportWidth
        {
            get
            {
                return mGraphicsDevice.Viewport.Width;
            }
        }

        /// <summary>
        /// Height (in pixels) of the camera's visible area
        /// </summary>
        public static int ViewportHeight
        {
            get
            {
                return mGraphicsDevice.Viewport.Height;
            }
        }

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
            Random = new Random();
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
