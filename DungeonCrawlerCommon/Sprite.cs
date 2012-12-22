using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scott.dungeon
{
    /// <summary>
    /// The sprite class is the base class used for rendering animated 2d images on
    /// screen. It uses a flywheel pattern to make sprites lightweight, where the sprite
    /// class itself stores current animation information, and references all the static
    /// sprite data from the SpriteData class.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData mSpriteData;

        /// <summary>
        /// The animation that is currently playing
        /// </summary>
        private AnimationData mCurrentAnimation;

        /// <summary>
        /// The current frame being animated
        /// </summary>
        private int mCurrentFrame;

        /// <summary>
        /// The time that the current frame was first displayed
        /// </summary>
        private TimeSpan mFrameStartTime;

        /// <summary>
        /// The current texture that should be displayed for the sprite
        /// </summary>
        public Texture2D CurrentTexture { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteData">The sprite data that this sprite will be rendering</param>
        public Sprite( SpriteData spriteData )
        {
            mSpriteData = spriteData;
        }
    }
}
