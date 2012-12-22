using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scott.dungeon
{
    /// <summary>
    /// An exception that happens during sprite animation
    /// </summary>
    public class AnimationException : Exception
    {
        public Sprite Sprite { get; private set; }
        public string AnimationName { get; private set; }
        public int AnimationFrame { get; private set; }

        public AnimationException( string message, Sprite sprite, string animationName, int animationFrame )
            : base( message )
        {
            Sprite = sprite;
            AnimationName = animationName;
            AnimationFrame = animationFrame;
        }
    }
}
