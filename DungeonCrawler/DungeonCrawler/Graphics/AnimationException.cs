using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scott.Dungeon.Graphics
{
    /// <summary>
    /// An exception that happens during sprite animation
    /// </summary>
    public class AnimationException : Exception
    {
        public string SpriteName { get; private set; }
        public string AnimationName { get; private set; }
        public int AnimationFrame { get; private set; }

        public AnimationException( string message, string spriteName, string animationName, int animationFrame )
            : base( message )
        {
            SpriteName = spriteName;
            AnimationName = animationName;
            AnimationFrame = animationFrame;
        }
    }
}
