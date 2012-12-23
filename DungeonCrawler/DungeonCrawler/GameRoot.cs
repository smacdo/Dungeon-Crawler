using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Scott.Dungeon.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon
{
    /// <summary>
    /// Global singleton that stores global systems, processors and the game object
    /// collection
    /// </summary>
    public static class GameRoot
    {
        private static bool mCreated = false;
        public static DebugRenderer Debug { get; private set; }

        /// <summary>
        /// Initialize the game root
        /// </summary>
        public static void Initialize( GraphicsDevice graphics, ContentManager content )
        {
            mCreated = true;
            Debug = new DebugRenderer( graphics, content );
        }

        public static void Unload()
        {
            Debug.Unload();
            Debug = null;
        }
    }
}
