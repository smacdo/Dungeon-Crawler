using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scott.dungeon
{
    /// <summary>
    /// Renderable sprite that uses a backing SpriteData to perform animations
    /// and other nifty things
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Sprite data that this sprite is using
        /// </summary>
        private SpriteData<Texture2D> mSpriteData;

        /// <summary>
        /// The animation that is currently playing
        /// </summary>
        private SpriteAnimationData mCurrentAnimation;

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
        /// <param name="spriteData">The animation data for this sprite</param>
        public Sprite( SpriteData<Texture2D> spriteData )
        {
            mSpriteData = spriteData;
        }
    }
}
